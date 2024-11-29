Prerequisite:

1. You have registered your fingerprint on the device that is running the solution (e.g. via Windows Hello).


Start the solution:

1. Open the solution in Visual Studio 2022
2. Restore Nuget packages
3. Run `update-database` in the Package Manager Console to create the localdb.
4. Start the application.

Register an account:

1. Click on Register in the top right corner
2. Fill in an email and strong password
3. Click "Register"

Testing biometric authentication:

1. Click on Login in the top right corner
2. Enter your credentials and click "Log in"
3. Click on the "1. Register fingerprint" button and follow the prompts
4. Click on the "2. Verify fingerprint" button and follow the prompts
5. You should see a pop up saying "Verification successful!" indicating you have been verified.
