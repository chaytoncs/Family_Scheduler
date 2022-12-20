# Family_Scheduler
### Logins are also accessible in FamilyScheduler>Areas>Identity>Data>InitializeUserRoles
#### Admin User
* Username: admin@chayton.info   Password: Testing123$

#### Household Members
* Username: member@chayton.info   Password: Testing123$
* Username: member2@chayton.info   Password: Testing123$
* Username: member3@chayton.info   Password: Testing123$
* Username: member4@chayton.info   Password: Testing123$

#### Final Note
On startup there will only be Users, Tasks, Workloads, Frequencies, and TaskTypes in the database. I have added enough data to quickly generate multiple assignments for every household member on startup, but you will need to run the Generate New Schedule to have assignments added.

If you choose to register a new user (Sign Up), the new user will be given the role of "Member" by default. The only way to add an Admin user is throught the code (in InitializeUserRoles.cs).

#### Steps to Generating a Schedule
1. Login as the Admin User.
2. Naviagate to the Schedule Tab in the Navigation Bar.
3. On the Schedule List view, click the Generate New Schedule Button.
4. On the Generate Schedule Form, input a number for Max Assignments (around 10 should be enough) and select a Start date.
5. Once you have the input fields filled out you can click the Generate Schedule button, which will create the Assignments.
