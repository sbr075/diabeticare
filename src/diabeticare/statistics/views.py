from flask import request, jsonify

from diabeticare import db
from diabeticare.statistics import bp
from diabeticare.statistics import BGLForm, SleepForm, CIForm
from diabeticare.statistics.models import BGL, Sleep, CI
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
		note:       (optional) note written by user (max 256 characters)
		identifier: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = request.get_json()
		username   = data["username"]
		timestamp  = data["timestamp"]

		value      = data["value"]
		note       = data["note"] if "note" in data else None
		identifier = data["identifier"] if "identifier" in data else None
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"RESPONSE": "Invalid token"})

		bgl_form = BGLForm(obj={"measurement": value, "date": timestamp, "note": note})
		if bgl_form.validate():
			if identifier:
				entry = BGL.query.filter(BGL.user==user, BGL.id==identifier)
				if not entry:
					return jsonify({"RESPONSE": "Invalid user"})
				
				entry.measurement = value
				entry.note = note
				entry.date = timestamp
			
			else:
				entry = BGL(user=user, measurement=value, note=note, date=timestamp)
				db.session.add(entry)
			
			db.session.commit()

			# Update user token
			new_token = update_token(user)

			return jsonify({"RESPONSE": "Request complete", "CSRF-Token": new_token})

		else:
			return jsonify({"RESPONSE": bgl_form.errors})

	return jsonify({"RESPONSE": "Invalid request"})

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
		note:       (optional) note written by user (max 256 characters)
		identifier: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = request.get_json()
		username   = data["username"]

		start	   = data["start"]
		stop       = data["stop"]
		note       = data["note"] if "note" in data else None
		identifier = data["identifier"] if "identifier" in data else None
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"RESPONSE": "Invalid token"})
		
		sleep_form = SleepForm(obj={"start": start, "stop": stop, "note": note})
		if sleep_form.validate():
			if identifier:
				entry = Sleep.query.filter(Sleep.user==user, Sleep.id==identifier)
				if not entry:
					return jsonify({"RESPONSE": "Invalid user"})
				
				entry.start = start
				entry.stop  = stop
				entry.note  = note
			
			else:
				entry = Sleep(user=user, start=start, stop=stop, note=note)
				db.session.add(entry)
			
			db.session.commit()

			# Update user token
			new_token = update_token(user)

			return jsonify({"RESPONSE": "Request complete", "CSRF-Token": new_token})

	return jsonify({"RESPONSE": "Invalid request"})


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
		note:       (optional) note written by user (max 256 characters)
		identifier: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = request.get_json()
		username   = data["username"]
		timestamp  = data["timestamp"]

		value      = data["value"]
		note       = data["note"] if "note" in data else None
		identifier = data["identifier"] if "identifier" in data else None
		token      = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"RESPONSE": "Invalid token"})
		
		ci_form = CIForm(obj={"carbohydrates": value, "date": timestamp, "note": note})
		if ci_form.validate():
			if identifier:
				entry = CI.query.filter(CI.user==user, CI.id==identifier)
				if not entry:
					return jsonify({"RESPONSE": "Invalid user"})
				
				entry.carbohydrates = value
				entry.date = timestamp
				entry.note = note

			else:
				entry = CI(user=user, carobohydrates=value, date=timestamp, note=note)
				db.session.add(entry)

			db.session.commit()

			# Update user token
			new_token = update_token(user)

			return jsonify({"RESPONSE": "Request complete", "CSRF-Token": new_token})				

	return jsonify({"RESPONSE": "Invalid request"})


@bp.route("/bgl/get", methods=["GET"])
def bgl_get():
	pass


@bp.route("/sleep/get", methods=["GET"])
def sleep_get():
	pass


@bp.route("/ci/get", methods=["GET"])
def ci_get():
	pass