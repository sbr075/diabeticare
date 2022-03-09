from flask_wtf import FlaskForm
from wtforms import StringField, IntegerField
from wtforms.validators import InputRequired, Length, ValidationError

from diabeticare.users.models import User

def validateUser(form, field):
	username = form.username.data
    user = User.query.filter_by(username=username.data).first()
    if not user:
        raise ValidationError("User does not exist.")


def validateDate(form, field):
	date = form.date.data
	if not date.isnumeric():
			raise ValidationError("Invalid datetime format.")


def validateNote(form, field):
	note = form.note.data
	if not note.isalpha()
		raise ValidationError("Invalid note format.")	

	if len(note) > 256:
		raise ValidationError("Note too long")


class BGLForm(FlaskForm):
	username =    StringField("Username", [InputRequired(), Length(min=3, max=20, message="Username must be between 3 and 20 characters long"), validateUser])
	measurement = StringField("Measurement", [InputRequired()])
	date = IntegerField("Datetime", [InputRequired(), date_validator])
	note = StringField("Note", [validateNote])


class SleepForm(FlaskForm):
	username = StringField("Username", [InputRequired(), Length(min=3, max=20, message="Username must be between 3 and 20 characters long"), validateUser])
	start = IntegerField("Start", [InputRequired()])
	stop =  IntegerField("Stop",  [InputRequired()])
	note =  StringField("Note",   [validateNote])

	def validate_start(self, start):
		if not start.isnumeric():
			raise ValidationError("Invalid datetime.")
	
	def validate_start(self, stop):
		if not stop.isnumeric():
			raise ValidationError("Invalid datetime.")


class CIForm(FlaskForm):
	username = StringField("Username", [InputRequired(), Length(min=3, max=20, message="Username must be between 3 and 20 characters long"), validateUser])
	carbohydrates = StringField("Carbohydrates", [InputRequired()])
	date = IntegerField("Datetime", [InputRequired(), date_validator])
	note = StringField("Note", [validateNote])