from flask_wtf import FlaskForm
from wtforms import StringField, IntegerField
from wtforms.validators import InputRequired, ValidationError

def validateDate(form, field):
	date = form.date.data
	if not date.isnumeric():
		raise ValidationError("Invalid datetime format.")


def validateNote(form, field):
	note = form.note.data
	if not note.isalpha():
		raise ValidationError("Invalid note format.")	

	if len(note) > 256:
		raise ValidationError("Note too long")


class BGLForm(FlaskForm):
	measurement = StringField("Measurement", [InputRequired()])
	date = IntegerField("Datetime", [InputRequired(), validateDate])
	note = StringField("Note", [validateNote])

	def validate_measurement(self, measurement):
		if not isinstance(measurement, float):
			raise ValidationError("Invalid format")


class SleepForm(FlaskForm):
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
	carbohydrates = StringField("Carbohydrates", [InputRequired()])
	date = IntegerField("Datetime", [InputRequired(), validateDate])
	note = StringField("Note", [validateNote])

	def validate_carbohydrates(self, carbohydrates):
		if not isinstance(carbohydrates, float):
			raise ValidationError("Invalid format")