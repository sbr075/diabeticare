import json
from flask import request, jsonify
from werkzeug.datastructures import MultiDict

from diabeticare import db
from diabeticare.statistics import bp
from diabeticare.statistics.forms import BGLForm, SleepForm, CIForm
from diabeticare.statistics.models import BGL, BGLSchema, Sleep, SleepSchema, CI, CISchema
from diabeticare.users.models import User
from diabeticare.main.views import validate_token, update_token

import logging
logging.basicConfig(level=logging.INFO, format="[%(asctime)s.%(msecs)03d] %(name)s:%(message)s", datefmt="%H:%M:%S")
logger = logging.getLogger("STAT")


@bp.route("/bgl/set", methods=["POST"])
def bgl_set():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:   name of user

		value:		value of entry
		timestamp:  time when measurement was taken (UNIXTIMESTAMP)
		identifier: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username   = data["username"]
		timestamp  = data["timestamp"]

		value      = data["value"]
		identifier = data["identifier"] if data["identifier"] >= 0 else None
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 403

		bgl_form = BGLForm(MultiDict({"measurement": value, "timestamp": timestamp}))
		if bgl_form.validate():
			if identifier:
				entry = BGL.query.filter(BGL.user_id==user.id, BGL.id==identifier).first()
				if not entry:
					return jsonify({"ERROR": "Invalid paramaters"})
				
				entry.measurement = value
				entry.timestamp = timestamp

			else:
				entry = BGL(user_id=user.id, measurement=value, timestamp=timestamp)
				db.session.add(entry)
			
			db.session.commit()

			# Update user token
			new_token = update_token(user)

			return jsonify({"X-CSRFToken": new_token, "SERVERID": entry.id})

		else:
			return jsonify({"ERROR": bgl_form.errors}), 401

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/bgl/get", methods=["GET"])
def bgl_get():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:   name of user
		timestamp:  time when measurement was taken (UNIXTIMESTAMP)
	"""
	
	if request.method == "GET":
		data = json.loads(request.data)
		username  = data["username"]
		timestamp = data["timestamp"]
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 403
		
		entries = BGL.query.filter(BGL.user_id==user.id, BGL.timestamp>=timestamp).all()
		if entries:
			bgl_schema = BGLSchema(many=True)
			results = bgl_schema.dump(entries)
		else:
			results = []

		# Update user token
		new_token = update_token(user)

		return jsonify({"RESULTS": results, "X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/bgl/del", methods=["POST"])
def bgl_del():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:   name of user
		identifier: id of entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username   = data["username"]
		identifier = data["identifier"]
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 403
		
		entry = BGL.query.filter(BGL.user_id==user.id, BGL.id==identifier)
		if not entry.first():
			return jsonify({"ERROR": "Invalid paramaters"})

		entry.delete()

		db.session.commit()

		# Update user token
		new_token = update_token(user)

		return jsonify({"X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/sleep/set", methods=["POST"])
def sleep_set():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:   name of user

		start:		time when user went to sleep (UNIXTIMESTAMP)
		stop:		time when user woke up  	 (UNIXTIMESTAMP)
		identifier: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username   = data["username"]

		start	   = data["start"]
		stop       = data["stop"]
		identifier = data["identifier"] if data["identifier"] >= 0 else None
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 403
		
		sleep_form = SleepForm(MultiDict({"start": start, "stop": stop}))
		if sleep_form.validate():
			if identifier:
				entry = Sleep.query.filter(Sleep.user_id==user.id, Sleep.id==identifier).first()
				if not entry:
					return jsonify({"ERROR": "Invalid paramaters"})
				
				entry.start = start
				entry.stop  = stop
			
			else:
				entry = Sleep(user_id=user.id, start=start, stop=stop)
				db.session.add(entry)
			
			db.session.commit()

			# Update user token
			new_token = update_token(user)

			return jsonify({"X-CSRFToken": new_token, "SERVERID": entry.id})

		else:
			return jsonify({"ERROR": sleep_form.errors}), 401

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/sleep/get", methods=["GET"])
def sleep_get():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:   name of user
		timestamp:  time when measurement was taken (UNIXTIMESTAMP)
	"""

	if request.method == "GET":
		data = json.loads(request.data)
		username  = data["username"]
		timestamp = data["timestamp"]
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 403
		
		entries = Sleep.query.filter(Sleep.user_id==user.id, Sleep.start>=timestamp).all()
		if entries:
			sleep_schema = SleepSchema(many=True)
			results = sleep_schema.dump(entries)
		else:
			results = []

		# Update user token
		new_token = update_token(user)

		return jsonify({"RESULTS": results, "X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/sleep/del", methods=["POST"])
def sleep_del():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:   name of user
		identifier: id of entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username   = data["username"]
		identifier = data["identifier"]
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 403
		
		entry = Sleep.query.filter(Sleep.user_id==user.id, Sleep.id==identifier)
		if not entry.first():
			return jsonify({"ERROR": "Invalid paramaters"})
			
		entry.delete()
		db.session.commit()

		# Update user token
		new_token = update_token(user)

		return jsonify({"X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/ci/set", methods=["POST"])
def ci_set():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:   name of user

		value:		value of entry
		timestamp:  time when measurement was taken (UNIXTIMESTAMP)
		identifier: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username   = data["username"]
		timestamp  = data["timestamp"]

		value      = data["value"]
		identifier = data["identifier"] if data["identifier"] >= 0 else None
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 403
		
		ci_form = CIForm(MultiDict({"carbohydrates": value, "timestamp": timestamp}))
		if ci_form.validate():
			if identifier:
				entry = CI.query.filter(CI.user_id==user.id, CI.id==identifier).first()
				if not entry:
					return jsonify({"ERROR": "Invalid paramaters"})
				
				entry.carbohydrates = value
				entry.timestamp = timestamp

			else:
				entry = CI(user_id=user.id, carbohydrates=value, timestamp=timestamp)
				db.session.add(entry)

			db.session.commit()

			# Update user token
			new_token = update_token(user)

			return jsonify({"X-CSRFToken": new_token, "SERVERID": entry.id})

		else:
			return jsonify({"ERROR": ci_form.errors}), 401			

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/ci/get", methods=["GET"])
def ci_get():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:   name of user
		timestamp:  time when measurement was taken (UNIXTIMESTAMP)
	"""

	if request.method == "GET":
		data = json.loads(request.data)
		username  = data["username"]
		timestamp = data["timestamp"]
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 403
		
		entries = CI.query.filter(CI.user_id==user.id, CI.timestamp>=timestamp).all()
		if entries:
			ci_schema = CISchema(many=True)
			results = ci_schema.dump(entries)
		else:
			results = []

		# Update user token
		new_token = update_token(user)

		return jsonify({"RESULTS": results, "X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/ci/del", methods=["POST"])
def ci_del():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:   name of user
		identifier: id of entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username   = data["username"]
		identifier = data["identifier"]
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 403

		entry = CI.query.filter(CI.user_id==user.id, CI.id==identifier)
		if not entry.first():
			return jsonify({"ERROR": "Invalid paramaters"})
		
		entry.delete()
		db.session.commit()

		# Update user token
		new_token = update_token(user)

		return jsonify({"X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405