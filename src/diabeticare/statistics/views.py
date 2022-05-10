import json
from unicodedata import name
from flask import request, jsonify
from werkzeug.datastructures import MultiDict

from diabeticare import db
from diabeticare.statistics import bp
from diabeticare.statistics.forms import BGLForm, SleepForm, CIForm, MoodForm, ExerciseForm
from diabeticare.statistics.models import BGL, BGLSchema, Sleep, SleepSchema, CI, CISchema, Mood, MoodSchema, Exercise, ExerciseSchema
from diabeticare.users.models import User
from diabeticare.main.views import validate_token, update_token

import logging
logging.basicConfig(level=logging.INFO, format="[%(asctime)s.%(msecs)03d] %(name)s:%(message)s", datefmt="%H:%M:%S")
logger = logging.getLogger("STAT")


@bp.route("/bgl/set", methods=["POST"])
def bglSet():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user

		value:	   value of entry
		timestamp: time when measurement was taken (UNIXTIMESTAMP)
		server_id: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		value     = data["value"]
		timestamp = data["timestamp"]
		server_id = data["server_id"] if data["server_id"] >= 0 else None
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498

		bgl_form = BGLForm(MultiDict({"measurement": value, "timestamp": timestamp}))
		if bgl_form.validate():
			if server_id:
				entry = BGL.query.filter(BGL.user_id==user.id, BGL.id==server_id).first()
				if not entry:
					return jsonify({"ERROR": "Invalid parameters"}), 401
				
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
def bglGet():
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
			return jsonify({"ERROR": "Invalid token"}), 498
		
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
def bglDel():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user
		server_id: id of entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		server_id = data["server_id"]
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498
		
		entry = BGL.query.filter(BGL.user_id==user.id, BGL.id==server_id)
		if not entry.first():
			return jsonify({"ERROR": "Invalid parameters"}), 401

		entry.delete()

		db.session.commit()

		# Update user token
		new_token = update_token(user)

		return jsonify({"X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/sleep/set", methods=["POST"])
def sleepSet():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user

		start:	   time when user went to sleep (UNIXTIMESTAMP)
		stop:	   time when user woke up  	 (UNIXTIMESTAMP)
		server_id: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		start	  = data["start"]
		stop      = data["stop"]
		server_id = data["server_id"] if data["server_id"] >= 0 else None
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498
		
		sleep_form = SleepForm(MultiDict({"start": start, "stop": stop}))
		if sleep_form.validate():
			if server_id:
				entry = Sleep.query.filter(Sleep.user_id==user.id, Sleep.id==server_id).first()
				if not entry:
					return jsonify({"ERROR": "Invalid parameters"}), 401
				
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
def sleepGet():
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
			return jsonify({"ERROR": "Invalid token"}), 498
		
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
def sleepDel():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user
		server_id: id of entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		server_id = data["server_id"]
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498
		
		entry = Sleep.query.filter(Sleep.user_id==user.id, Sleep.id==server_id)
		if not entry.first():
			return jsonify({"ERROR": "Invalid parameters"}), 401
			
		entry.delete()
		db.session.commit()

		# Update user token
		new_token = update_token(user)

		return jsonify({"X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/ci/set", methods=["POST"])
def ciSet():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user

		value:	   value of entry
		timestamp: time when measurement was taken (UNIXTIMESTAMP)
		server_id: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		value     = data["value"]
		name	  = data["name"]
		timestamp = data["timestamp"]
		server_id = data["server_id"] if data["server_id"] >= 0 else None
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498
		
		ci_form = CIForm(MultiDict({"carbohydrates": value, "name": name, "timestamp": timestamp}))
		if ci_form.validate():
			if server_id:
				entry = CI.query.filter(CI.user_id==user.id, CI.id==server_id).first()
				if not entry:
					return jsonify({"ERROR": "Invalid parameters"}), 401
				
				entry.carbohydrates = value
				entry.timestamp = timestamp

			else:
				entry = CI(user_id=user.id, carbohydrates=value, name=name, timestamp=timestamp)
				db.session.add(entry)

			db.session.commit()

			# Update user token
			new_token = update_token(user)

			return jsonify({"X-CSRFToken": new_token, "SERVERID": entry.id})

		else:
			return jsonify({"ERROR": ci_form.errors}), 401			

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/ci/get", methods=["GET"])
def ciGet():
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
			return jsonify({"ERROR": "Invalid token"}), 498
		
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
def ciDel():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user
		server_id: id of entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		server_id = data["server_id"]
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498

		entry = CI.query.filter(CI.user_id==user.id, CI.id==server_id)
		if not entry.first():
			return jsonify({"ERROR": "Invalid parameters"}), 401
		
		entry.delete()
		db.session.commit()

		# Update user token
		new_token = update_token(user)

		return jsonify({"X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/mood/set", methods=["POST"])
def moodSet():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user

		value:	   value of entry
		timestamp: time when measurement was taken (UNIXTIMESTAMP)
		server_id: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		value     = data["value"]
		timestamp = data["timestamp"]
		server_id = data["server_id"] if data["server_id"] >= 0 else None
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498
		
		mood_form = MoodForm(MultiDict({"mood": value, "timestamp": timestamp}))
		if mood_form.validate():
			if server_id:
				entry = Mood.query.filter(Mood.user_id==user.id, Mood.id==server_id).first()
				if not entry:
					return jsonify({"ERROR": "Invalid parameters"}), 401
				
				entry.mood = value
				entry.timestamp = timestamp

			else:
				entry = Mood(user_id=user.id, mood=value, timestamp=timestamp)
				db.session.add(entry)

			db.session.commit()

			# Update user token
			new_token = update_token(user)

			return jsonify({"X-CSRFToken": new_token, "SERVERID": entry.id})

		else:
			return jsonify({"ERROR": mood_form.errors}), 401	

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/mood/get", methods=["GET"])
def moodGet():
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
			return jsonify({"ERROR": "Invalid token"}), 498
		
		entries = Mood.query.filter(Mood.user_id==user.id, Mood.timestamp>=timestamp).all()
		if entries:
			mood_schema = MoodSchema(many=True)
			results = mood_schema.dump(entries)
		else:
			results = []

		# Update user token
		new_token = update_token(user)

		return jsonify({"RESULTS": results, "X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/mood/del", methods=["POST"])
def moodDel():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user
		server_id: id of entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		server_id = data["server_id"]
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498

		entry = Mood.query.filter(Mood.user_id==user.id, Mood.id==server_id)
		if not entry.first():
			return jsonify({"ERROR": "Invalid parameters"}), 401
		
		entry.delete()
		db.session.commit()

		# Update user token
		new_token = update_token(user)

		return jsonify({"X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/exercise/set", methods=["POST"])
def exerciseSet():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user

		name: 	   name of exercise
		start:	   time when user went to sleep (UNIXTIMESTAMP)
		stop:	   time when user woke up  	 (UNIXTIMESTAMP)
		server_id: (optional) used to overwrite existing entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		name	  = data["name"]
		start	  = data["start"]
		stop      = data["stop"]
		server_id = data["server_id"] if data["server_id"] >= 0 else None
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498
		
		exercise_form = ExerciseForm(MultiDict({"name": name, "start": start, "stop": stop}))
		if exercise_form.validate():
			if server_id:
				entry = Exercise.query.filter(Exercise.user_id==user.id, Exercise.id==server_id).first()
				if not entry:
					return jsonify({"ERROR": "Invalid parameters"}), 401
				
				entry.name = name
				entry.start = start
				entry.stop  = stop
			
			else:
				entry = Exercise(user_id=user.id, name=name, start=start, stop=stop)
				db.session.add(entry)
			
			db.session.commit()

			# Update user token
			new_token = update_token(user)

			return jsonify({"X-CSRFToken": new_token, "SERVERID": entry.id})

		else:
			return jsonify({"ERROR": exercise_form.errors}), 401

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/exercise/get", methods=["GET"])
def exerciseGet():
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

		name = data["name"]
		timestamp = data["timestamp"]
		token = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498
		
		entries = Exercise.query.filter(Exercise.user_id==user.id, Exercise.name==name, Exercise.start>=timestamp).all()
		if entries:
			exercise_schema = ExerciseSchema(many=True)
			results = exercise_schema.dump(entries)
		else:
			results = []

		# Update user token
		new_token = update_token(user)

		return jsonify({"RESULTS": results, "X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405


@bp.route("/exercise/del", methods=["POST"])
def exerciseDel():
	"""
	Request parameters
	headers
		X-CSRFToken: current valid token
	
	content/data (json format)
		username:  name of user
		server_id: id of entry
	"""

	if request.method == "POST":
		data = json.loads(request.data)
		username  = data["username"]
		server_id = data["server_id"]
		token     = request.headers["X-CSRFToken"]

		# Get user and check token validity
		user = User.query.filter_by(username=username).first()
		if not validate_token(user, token):
			return jsonify({"ERROR": "Invalid token"}), 498
		
		entry = Exercise.query.filter(Exercise.user_id==user.id, Exercise.id==server_id)
		if not entry.first():
			return jsonify({"ERROR": "Invalid parameters"}), 401
			
		entry.delete()
		db.session.commit()

		# Update user token
		new_token = update_token(user)

		return jsonify({"X-CSRFToken": new_token})

	return jsonify({"ERROR": "Invalid request"}), 405