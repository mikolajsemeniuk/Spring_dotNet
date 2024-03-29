# Add HTTPS

**OS X**

	1. Double click on the certificate (server.crt)
	2. Select your desired keychain (login should suffice)
	3. Add the certificate
	4. Open Keychain Access if it isn’t already open
	5. Select the keychain you chose earlier
	6. You should see the certificate localhost
	7. Double click on the certificate
	8. Expand Trust
	9. Select the option Always Trust in When using this certificate
	10. Close the certificate window

**Windows 10**

	1. Double click on the certificate (server.crt)
	2. Click on the button “Install Certificate …”
	3. Select whether you want to store it on user level or on machine level
	4. Click “Next”
	5. Select “Place all certificates in the following store”
	6. Click “Browse”
	7. Select “Trusted Root Certification Authorities”
	8. Click “Ok”
	9. Click “Next”
	10. Click “Finish”

If you get a prompt, click “Yes”

**Both**
Put `server.crt` and `server.key` in `root/ssl`

Then in `root/angular.json`
```json
// ...
"serve": {
  "builder": "@angular-devkit/build-angular:dev-server",
    "options": {
      // ADD THIS
      "sslCert": "./ssl/server.crt",
      // ADD THIS
      "sslKey": "./ssl/server.key",
      // ADD THIS
      "ssl": true,
      "browserTarget": "clientapp:build"
    },
    "configurations": {
      "production": {
        "browserTarget": "clientapp:build:production"
    }
  }
},
// ...
```
