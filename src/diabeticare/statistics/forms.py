from flask_wtf import FlaskForm
from wtforms import StringField, IntegerField, FloatField
from wtforms.validators import InputRequired, ValidationError, Length

# Validates input from application with forms

def validateDate(form, field):
	timestamp = form.timestamp.data
	if not isinstance(timestamp, int):
		raise ValidationError("Invalid datetime format.")


class BGLForm(FlaskForm):
	measurement = FloatField("Measurement", [InputRequired()])
	timestamp = IntegerField("Timestamp", [InputRequired(), validateDate])

	def validate_measurement(self, measurement):
		if not isinstance(measurement.data, float):
			raise ValidationError("Invalid format")


class SleepForm(FlaskForm):
	start = IntegerField("Start", [InputRequired()])
	stop =  IntegerField("Stop",  [InputRequired()])

	def validate_start(self, start):
		if not isinstance(start.data, int):
			raise ValidationError("Invalid format")
	
	def validate_stop(self, stop):
		if not isinstance(stop.data, int):
			raise ValidationError("Invalid format")


class CIForm(FlaskForm):
	carbohydrates = FloatField("Carbohydrates", [InputRequired()])
	name = StringField("Name", [InputRequired(), Length(max=100, message="Name cannot be longer than 100 characters")])
	timestamp = IntegerField("Timestamp", [InputRequired(), validateDate])

	def validate_carbohydrates(self, carbohydrates):
		if not isinstance(carbohydrates.data, float):
			raise ValidationError("Invalid format")
	
	def validate_name(self, name):
		if not isinstance(name.data, str):
			raise ValidationError("Invalid format")
		

class MoodForm(FlaskForm):
	mood = IntegerField("Mood", [InputRequired()])
	timestamp = IntegerField("Timestamp", [InputRequired(), validateDate])

	def validate_mood(self, mood):
		if not isinstance(mood.data, int):
			raise ValidationError("Invalid format")


class ExerciseForm(FlaskForm):
	name = StringField("Name", [InputRequired()])
	start = IntegerField("Start", [InputRequired()])
	stop =  IntegerField("Stop",  [InputRequired()])

	def validate_name(self, name):
		if not isinstance(name.name, str):
			raise ValidationError("Invalid format")

	def validate_start(self, start):
		if not isinstance(start.data, int):
			raise ValidationError("Invalid format")
	
	def validate_stop(self, stop):
		if not isinstance(stop.data, int):
			raise ValidationError("Invalid format")