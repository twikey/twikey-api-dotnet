build:
	dotnet build src/Twikey/Twikey.csproj

test:
	MNDTNUMBER=PS1ACT11273 CT=772 TWIKEY_API_KEY=073E5BBB0B23E6B1F6927D777DE3B44864F2B1E5 dotnet test --logger "console;verbosity=normal"  src/TwikeyTests

test-verbose:
	MNDTNUMBER=PS1ACT11273 CT=772 TWIKEY_API_KEY=073E5BBB0B23E6B1F6927D777DE3B44864F2B1E5 dotnet test --logger "console;verbosity=detailed"  src/TwikeyTests

test-tx:
	MNDTNUMBER=PS1ACT11273 CT=772 TWIKEY_API_KEY=073E5BBB0B23E6B1F6927D777DE3B44864F2B1E5 dotnet test --filter TransactionTest --logger "console;verbosity=detailed"  src/TwikeyTests

test-pl:
	MNDTNUMBER=PS1ACT11273 CT=772 TWIKEY_API_KEY=073E5BBB0B23E6B1F6927D777DE3B44864F2B1E5 dotnet test --filter PaylinkTest --logger "console;verbosity=detailed"  src/TwikeyTests

test-mandate:
	MNDTNUMBER=PS1ACT11273 CT=772 TWIKEY_API_KEY=073E5BBB0B23E6B1F6927D777DE3B44864F2B1E5 dotnet test --filter MandateTest --logger "console;verbosity=detailed"  src/TwikeyTests

test-invoice:
	MNDTNUMBER=PS1ACT11273 CT=772 TWIKEY_API_KEY=073E5BBB0B23E6B1F6927D777DE3B44864F2B1E5 dotnet test --filter InvoiceTest --logger "console;verbosity=detailed"  src/TwikeyTests
