# syntax=docker/dockerfile:1

FROM python:3.10.4-buster

WORKDIR /diabeticare-server-docker

COPY requirements.txt requirements.txt
RUN pip3 install -r requirements.txt

COPY . .

ENV FLASK_APP /diabeticare-server-docker/src/diabeticare

CMD ["python3", "-m", "flask", "run", "--cert=src/diabeticare/certs/cert.pem", "--key=src/diabeticare/certs/key.pem", "--host=0.0.0.0", "--port=8000"]
