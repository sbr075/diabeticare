from flask import Blueprint

bp = Blueprint('main', __name__)

from diabeticare.main import views