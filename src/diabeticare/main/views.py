from flask import request, jsonify
from flask_wtf.csrf import generate_csrf
import datetime

from diabeticare import db
from diabeticare.main import bp

# Called once upon registration/login to get an initial CSRF token
@bp.route("/get_token", methods=["GET"])
def get_token():
    if request.method == "GET":
        return jsonify({"RESPONSE": "X-CSRFToken generated", "X-CSRFToken": generate_csrf()})
    
    return jsonify({"RESPONSE": "Invalid request"})


def validate_token(user, token):
    return user.token == token and datetime.datetime.utcnow() <= user.timer


def update_token(user):
    new_token = generate_csrf()
    user.token = new_token
    user.timer = datetime.datetime.utcnow() + datetime.timedelta(minutes=5)
    db.session.commit()

    return new_token


def nullify_token(user):
    user.token = None
    user.timer = None
    db.session.commit()