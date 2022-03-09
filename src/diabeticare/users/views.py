from flask import request, jsonify
from passlib.hash import sha512_crypt
import datetime

from diabeticare import db
from diabeticare.users import bp
from diabeticare.users.forms import RegistrationForm, LoginForm, LogoutForm
from diabeticare.users.models import User
from diabeticare.main.views import validate_token, update_token, nullify_token

import logging
logging.basicConfig(level=logging.INFO, format="[%(asctime)s.%(msecs)03d] %(name)s:%(message)s", datefmt="%H:%M:%S")
logger = logging.getLogger("VALIDATOR")

@bp.route("/login", methods=["POST"])
def login():
    if request.method == "POST":
        data = request.get_json()

        login_form = LoginForm(obj=data)
        if login_form.validate():
            user = User.query.filter_by(username=login_form.username.data).first()
            csrf_token = update_token(user)

            return jsonify({"RESPONSE": "Successfully logged in", "CSRF-TOKEN": csrf_token})
        
        else:
            return jsonify({"RESPONSE": login_form.errors})
    
    return jsonify({"RESPONSE": "Invalid request"})


@bp.route("/logout", methods=["POST"])
def logout():
    if request.method == "POST":
        data = request.get_json()
        logger.info(data)

        logout_form = LogoutForm(obj=data)
        if logout_form.validate():
            user = User.query.filter_by(username=logout_form.username.data).first()
            token = request.headers["X-CSRFToken"]

            if not validate_token(user, token):
                return jsonify({"RESPONSE": "Invalid token"})

            nullify_token(user)
            return jsonify({"RESPONSE": "Successfully logged out"})

        else:
            return jsonify({"RESPONSE": logout_form.errors})

    return jsonify({"RESPONSE": "Invalid request"})


@bp.route("/register", methods=["POST"])
def register():
    if request.method == "POST":
        data = request.get_json()

        reg_form = RegistrationForm(obj=data)
        if reg_form.validate():
            hash_pwd = sha512_crypt.hash(reg_form.password.data)
            user = User(username=reg_form.username.data, email=reg_form.email.data, hash_pwd=hash_pwd)

            db.session.add(user)
            db.session.commit()

            return jsonify({"RESPONSE": "Account successfully created!"})
        
        else:
            return jsonify({"RESPONSE": reg_form.errors})
    
    return jsonify({"RESPONSE": "Invalid request"})