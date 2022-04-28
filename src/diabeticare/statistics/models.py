from diabeticare import db, ma

class BGL(db.Model):
    __tablename__ = "bgl"

    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)

    measurement = db.Column(db.Float, nullable=False)
    timestamp = db.Column(db.Integer, nullable=False)


class BGLSchema(ma.SQLAlchemySchema):
    class Meta:
        model = BGL
    
    measurement = ma.auto_field()
    timestamp = ma.auto_field()


class Sleep(db.Model):
    __tablename__ = "sleep"

    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)

    start = db.Column(db.Integer, nullable=False)
    stop  = db.Column(db.Integer, nullable=False)


class SleepSchema(ma.SQLAlchemyAutoSchema):
    class Meta:
        model = Sleep

    start = ma.auto_field()
    stop = ma.auto_field()


class CI(db.Model):
    __tablename__ = "carbohydrates"

    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)

    carbohydrates = db.Column(db.Float, nullable=False)
    name = db.Column(db.String(100), nullable=False)
    timestamp = db.Column(db.Integer, nullable=False)
    

class CISchema(ma.SQLAlchemyAutoSchema):
    class Meta:
        model = CI

    carbohydrates = ma.auto_field()
    name = ma.auto_field()
    timestamp = ma.auto_field()


class Mood(db.Model):
    __tablename__ = "mood"

    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)

    mood = db.Column(db.String(100), nullable=False)
    timestamp = db.Column(db.Integer, nullable=False)
    

class MoodSchema(ma.SQLAlchemyAutoSchema):
    class Meta:
        model = Mood

    mood = ma.auto_field()
    timestamp = ma.auto_field()


class Exercise(db.Model):
    __tablename__ = "exercise"

    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey("user.id"), nullable=False)

    name  = db.Column(db.String, nullable=False)
    start = db.Column(db.Integer, nullable=False)
    stop  = db.Column(db.Integer, nullable=False)


class ExerciseSchema(ma.SQLAlchemyAutoSchema):
    class Meta:
        model = Exercise

    name = ma.auto_field()
    start = ma.auto_field()
    stop = ma.auto_field()