from flask import request, jsonify
from werkzeug.datastructures import MultiDict

from diabeticare import db
from diabeticare.users import bp
from diabeticare.users.forms import RegistrationForm, LoginForm, LogoutForm
from diabeticare.users.models import User
from diabeticare.main.views import validate_token, update_token, nullify_token

import logging
import json

logging.basicConfig(level=logging.INFO, format="[%(asctime)s.%(msecs)03d] %(name)s:%(message)s", datefmt="%H:%M:%S")
logger = logging.getLogger("USER")

@bp.route("/login", methods=["POST"])
def login():
    """
	Request parameters
	username:    name of user
    password:    users password
	"""
    if request.method == "POST":
        data = json.loads(request.data)
        log_data = {
            "username": data["Username"],
            "password": data["Password"],
        }

        login_form = LoginForm(MultiDict(log_data))
        if login_form.validate():
            user = User.query.filter_by(username=login_form.username.data).first()
            new_token = update_token(user)

            return jsonify({"X-CSRFToken": new_token})
        
        else:
            return jsonify(login_form.errors), 401
    
    return jsonify({"RESPONSE": "Invalid request"}), 405


@bp.route("/logout", methods=["POST"])
def logout():
    """
	Request parameters
	username:    name of user
	X-CSRFToken: current valid token
	"""
    if request.method == "POST":
        data = json.loads(request.data)
        log_data = {
            "username": data["Username"]
        }

        logout_form = LogoutForm(MultiDict(log_data))
        if logout_form.validate():
            user = User.query.filter_by(username=logout_form.username.data).first()
            token = request.headers["X-CSRFToken"]

            if not validate_token(user, token):
                return jsonify({"RESPONSE": "Invalid token"})

            nullify_token(user)
            return jsonify({"RESPONSE": "Successfully logged out"})

        else:
            return jsonify(logout_form.errors), 401

    return jsonify({"RESPONSE": "Invalid request"}), 405


@bp.route("/register", methods=["POST"])
def register():
    """
	Request parameters
	username: name of user
    email:    users email
    password: users password
    confirm:  same as password
	"""
    
    if request.method == "POST":
        data = json.loads(request.data)
        reg_data = {
            "username": data["Username"],
            "email": data["Email"],
            "password": data["Password"],
            "confirm": data["ConfirmPassword"]
        }

        reg_form = RegistrationForm(MultiDict(reg_data))
        if reg_form.validate():
            user = User(username=reg_form.username.data, email=reg_form.email.data, hash_pwd=reg_form.password.data)

            db.session.add(user)
            db.session.commit()

            return jsonify({"RESPONSE": "Account successfully created!"})
        
        else:
            return jsonify(reg_form.errors), 401
    
    return jsonify({"RESPONSE": "Invalid request"}), 405