from flask import request, jsonify
from flask_wtf.csrf import generate_csrf
from passlib.hash import sha512_crypt
import datetime

from diabeticare import db
from diabeticare.users import bp
from diabeticare.users.forms import RegistrationForm, LoginForm
from diabeticare.users.models import User

import logging
logging.basicConfig(level=logging.INFO, format="[%(asctime)s.%(msecs)03d] %(name)s:%(message)s", datefmt="%H:%M:%S")
logger = logging.getLogger("VALIDATOR")

def validate_token(username, token):
    user = User.query.filter_by(username=username).first()
    logger.info(user.cookie)
    logger.info(token)
    return user.cookie == token and datetime.datetime.utcnow() <= user.timer


@bp.route("/login", methods=["POST"])
def login():
    if request.method == "POST":
        data = request.get_json()
        log_form = LoginForm(obj=data)

        if log_form.validate():
            csrf_token = generate_csrf()

            user = User.query.filter_by(username=log_form.username.data).first()
            user.cookie = csrf_token
            user.timer = datetime.datetime.utcnow() + datetime.timedelta(seconds=5)
            db.session.commit()

            return jsonify({"RESPONSE": "Successfully logged in", "CSRF-TOKEN": csrf_token})
        
        else:
            return jsonify({"RESPONSE": log_form.errors})
    
    return jsonify({"RESPONSE": "Invalid request"})


@bp.route("/logout", methods=["POST"])
def logout():
    if request.method == "POST":
        data = request.get_json()

        if not validate_token(data["username"], request.headers["X-CSRFToken"]):
            return jsonify({"RESPONSE": "Invalid token"})

        user = User.query.filter_by(username=data["username"]).first()
        user.cookie = None
        user.timer  = None
        db.session.commit()

        return jsonify({"RESPONSE": "Successfully logged out"})

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


# Called once upon registration/login to get an initial CSRF token
@bp.route("/get_cookie", methods=["GET"])
def get_cookie():
    if request.method == "GET":
        return jsonify({"RESPONSE": "CSRF-TOKEN successfully generated", "CSRF-TOKEN": generate_csrf()})
    
    return jsonify({"RESPONSE": "Invalid request"})