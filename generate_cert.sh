#!/bin/bash
openssl req -x509 -newkey rsa:4096 -nodes -out cert.pem -keyout key.pem -days 365
mkdir certs
mv cert.pem certs
mv key.pem certs
mv certs src/diabeticare
echo "Done"
