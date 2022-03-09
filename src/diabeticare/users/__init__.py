from flask import Blueprint

bp = Blueprint("users", __name__)

from diabeticare.users import views