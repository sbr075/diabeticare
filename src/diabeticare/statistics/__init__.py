from flask import Blueprint

bp = Blueprint("statistics", __name__)

from diabeticare.statistics import views