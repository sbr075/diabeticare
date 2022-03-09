from flask import request, jsonify
import datetime

from diabeticare import db
from diabeticare.statistics import bp
from diabeticare.statistics import BGLForm, SleepForm, CIForm
from diabeticare.statistics.models import BGL, Sleep, CI
from diabeticare.users.models import User
from diabeticare.main.views import validate_token, update_token, nullify_token

import logging
logging.basicConfig(level=logging.INFO, format="[%(asctime)s.%(msecs)03d] %(name)s:%(message)s", datefmt="%H:%M:%S")
logger = logging.getLogger("STAT")

# BLOOD GLUCOSE LEVELS
@bp.route("/bgl/get", methods=["GET"])
def get_bgl():
	"""
	Request parameters
	username:    name of user
	X-CSRFToken: current valid token
	timestamp:   last 30d/7d/1d (UNIXTIMESTAMP)
	"""
	if request.method == "GET":
		data = request.get_json()
		
		# Get timestamp
		ts = int(data["timestamp"])
		timestamp = datetime.utcfromtimestamp(ts).strftime('%Y-%m-%d')
		
		# Fetch user and token received
		user  = User.query.filter_by(username=data["username"]).first()
		token = request.headers["X-CSRFToken"]
		
		# Compare and check validity
		if not validate_token(user, token):
			return jsonify({"RESPONSE": "Invalid token"})

		# Fetch all sleep data after given timestamp
		sleep = BGL.query.filter_by(user=user, server_date>=timestamp)
		new_token = update_token(user)

		logger.info("sleep")

		return jsonify({"RESPONSE": "Sleep data", "CSRF-TOKEN": new_token})

	return jsonify({"RESPONSE": "Invalid request"})

@bp.route("/bgl/set", methods=["POST"])
def set_bgl():
	"""
	Creates or updates existing entry

	Request parameters
	username:    name of user
	X-CSRFToken: current valid token
	measurement: measured bgl
	timestamp:   time when measurement was taken (UNIXTIMESTAMP)
	"""
	if request.method == "POST":
		data = request.get_json()
		
		# Get timestamp
		ts = int(data["timestamp"])
		timestamp = datetime.utcfromtimestamp(ts)



	return jsonify({"RESPONSE": "Invalid request"})

@bp.route("/bgl/delete", methods=["POST"])
def del_bgl():
	if request.method == "POST":
		data = request.get_json()
		username = data["username"]

	return jsonify({"RESPONSE": "Invalid request"})


# SLEEP
@bp.route("/sleep/get", methods=["GET"])
def get_sleep():
	if request.method == "GET":
		data = request.get_json()

	return jsonify({"RESPONSE": "Invalid request"})

@bp.route("/sleep/set", methods=["POST"])
def set_sleep():
	if request.method == "POST":
		data = request.get_json()

	return jsonify({"RESPONSE": "Invalid request"})

@bp.route("/sleep/delete", methods=["POST"])
def del_sleep():
	if request.method == "POST":
		data = request.get_json()

	return jsonify({"RESPONSE": "Invalid request"})


# CARBOHYDRATE INTAKE
@bp.route("/ci/get", methods=["GET"])
def get_ci():
	if request.method == "GET":
		data = request.get_json()

	return jsonify({"RESPONSE": "Invalid request"})

@bp.route("/ci/set", methods=["POST"])
def set_ci():
	if request.method == "POST":
		data = request.get_json()

	return jsonify({"RESPONSE": "Invalid request"})

@bp.route("/ci/delete", methods=["POST"])
def del_ci():
	if request.method == "POST":
		data = request.get_json()

	return jsonify({"RESPONSE": "Invalid request"})