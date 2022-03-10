from flask_wtf import FlaskForm
from wtforms import StringField, IntegerField
from wtforms.validators import InputRequired, ValidationError


import logging
logging.basicConfig(level=logging.INFO, format="[%(asctime)s.%(msecs)03d] %(name)s:%(message)s", datefmt="%H:%M:%S")
logger = logging.getLogger("STAT")


def validateDate(form, field):
	date = form.date.data
	if not isinstance(date, int):
		raise ValidationError("Invalid datetime format.")


def validateNote(form, field):
	note = form.note.data
	if not note:
		return

	if not note.isalpha():
		raise ValidationError("Invalid note format.")	

	if len(note) > 256:
		raise ValidationError("Note too long")


class BGLForm(FlaskForm):
	measurement = StringField("Measurement", [InputRequired()])
	date = IntegerField("Date", [InputRequired(), validateDate])
	note = StringField("Note", [validateNote])

	def validate_measurement(self, measurement):
		try:
			float(measurement.data)
		except:
			raise ValidationError("Invalid format")


class SleepForm(FlaskForm):
	start = IntegerField("Start", [InputRequired()])
	stop =  IntegerField("Stop",  [InputRequired()])
	note =  StringField("Note",   [validateNote])

	def validate_start(self, start):
		if not isinstance(start.data, int):
			raise ValidationError("Invalid datetime.")
	
	def validate_start(self, stop):
		if not isinstance(stop.data, int):
			raise ValidationError("Invalid datetime.")


class CIForm(FlaskForm):
	carbohydrates = IntegerField("Carbohydrates", [InputRequired()])
	date = IntegerField("Date", [InputRequired(), validateDate])
	note = StringField("Note", [validateNote])

	def validate_carbohydrates(self, carbohydrates):
		if not isinstance(carbohydrates.data, int):
			raise ValidationError("Invalid format")