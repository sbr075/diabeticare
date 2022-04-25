from flask_wtf import FlaskForm
from wtforms import StringField, IntegerField
from wtforms.validators import InputRequired, ValidationError

def validateDate(form, field):
	timestamp = form.timestamp.data
	if not isinstance(timestamp, int):
		raise ValidationError("Invalid datetime format.")


class BGLForm(FlaskForm):
	measurement = StringField("Measurement", [InputRequired()])
	timestamp = IntegerField("Timestamp", [InputRequired(), validateDate])

	def validate_measurement(self, measurement):
		try:
			float(measurement.data)
		except:
			raise ValidationError("Invalid format")


class SleepForm(FlaskForm):
	start = IntegerField("Start", [InputRequired()])
	stop =  IntegerField("Stop",  [InputRequired()])

	def validate_start(self, start):
		if not isinstance(start.data, int):
			raise ValidationError("Invalid datetime.")
	
	def validate_start(self, stop):
		if not isinstance(stop.data, int):
			raise ValidationError("Invalid datetime.")


class CIForm(FlaskForm):
	carbohydrates = IntegerField("Carbohydrates", [InputRequired()])
	timestamp = IntegerField("Timestamp", [InputRequired(), validateDate])

	def validate_carbohydrates(self, carbohydrates):
		if not isinstance(carbohydrates.data, int):
			raise ValidationError("Invalid format")