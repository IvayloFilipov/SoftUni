const UserModel = firebase.auth();
const DB = firebase.firestore();

// ----- main logic -----
const app = Sammy('#root', function () {
    this.use('Handlebars', 'hbs');

    // --- load (homePage.hbs) ---
    this.get('/home', function (context) {

        DB.collection('allDestinations')
            .get()
            .then(response => {
                //console.log(response) // <- the response has property docs: witch is Array and that is why I can use map(...)
                context.allDestinations = response.docs.map(currDest => {
                    return { id: currDest.id, ...currDest.data() } //currDest.id -> comes from DB, this is  the documentNumber-someID, created by firebase when the movie is created/added into the DB
                });
                //console.log(context.allDestinations); // <- return all Dest with all properties
                extendContext(context)
                    .then(function () {
                        this.partial('/templates/homePage.hbs');
                    });
            })
            .catch(handleError)
    });

    // --- USER ---
    // - load loginForm.hbs -
    this.get('/loadLoginForm', function (context) {
        extendContext(context)
            .then(function (context) {
                this.partial('/templates/loginForm.hbs');
            });
    });
    // - login existing user - POST request
    this.post('/loginUser', function (context) {
        const { email, password } = context.params;

        //console.log(context.params)
        if (email === '') {
            throw new Error('Email field must not be empty!');
            //return;
        };
        if (password.length < 6 || password.length === '') {
            throw new Error('Invalid password !');
            //return;
        };

        UserModel.signInWithEmailAndPassword(email, password)
            .then(userData => {
                saveUserDataInLocaleStorage(userData);
                //successActionNote('Logged in Successfully!');
                this.redirect('/home')
            })
            .catch(handleError)
    });
    // - logOut user -
    this.get('/logoutUser', function (context) {
        UserModel.signOut()
            .then(userData => {
                removeUserDataFromLocaleStorage(userData);
                this.redirect('/home');
            })
            .catch(handleError)
    });

    // - load registerForm.hbs -
    this.get('/loadRegisterForm', function (context) {
        extendContext(context)
            .then(function (context) {
                this.partial('/templates/registerForm.hbs');
            });
    });
    // - sent/register user -
    this.post('/registerUser', function (context) {
        const { email, password, rePassword } = context.params;
        //console.log(context.params)

        if (email.length === '') {
            throw new Error('Email field must not be empty!');
            //return;
        };
        if (password.length < 6 || rePassword.length < 6) {
            throw new Error('Password and Repeat password length must be the at least 6 characters!');
            //return;
        };
        if (password !== rePassword) {
            throw new Error('Password and Repeat password must be the same!');
            //return;
        };

        UserModel.createUserWithEmailAndPassword(email, password)
            .then(userData => {
                //successActionNote('Successful registration!')
                saveUserDataInLocaleStorage(userData);
                this.redirect('/home')
            })
            .catch(handleError);
    });

    // --- Destinations ---
    // - load addDestinationForm.hbs
    this.get('/loadAddDestForm', function (context) {
        extendContext(context)
            .then(function (context) {
                this.partial('/templates/addDestinationForm.hbs');
            });
    });
    // - create/send new destination into DB - POST request
    this.post('/createDestination', function (context) {
        const { destination, city, duration, departureDate, imgUrl } = context.params;

        if (destination === '' || city === '' || duration === '' || departureDate === '' || imgUrl === '') {
            console.log('Input fields must not be empty!')
            return;
        };

        const newDestination = {
            destination,
            city,
            duration,
            departureDate,
            imgUrl,
            creator: getUserDataFromLocaleStorage().uid,
        };

        DB.collection('allDestinations')
            .add(newDestination)
            .then(response => {
                this.redirect('/home')
            })
            .catch(handleError)
    });
    // - load curr dest details -
    this.get('/loadCurrDestDetails/:destId', function (context) {
        const { destId } = context.params;
        //console.log(destId)

        DB.collection('allDestinations')
            .doc(destId)
            .get()
            .then(response => {
                const { email, uid } = getUserDataFromLocaleStorage();

                // NO need to have this below into destinationDetails.hbs
                //const isCreator = Boolean(uid === response.data().creator);

                // NO need to have this below into destinationDetails.hbs
                // const alreadyLikedMovie = Boolean(response.data().peopleLiked.find((currEmail) => currEmail === email));

                //context.currDest = { ...response.data(), id: destId, isCreator };

                context.currDest = { ...response.data(), id: destId };

                extendContext(context)
                    .then(function (context) {
                        this.partial('/templates/destinationDetails.hbs')
                    })
            })
            .catch(handleError)
    });
    // - load currDest EDIT form -
    this.get('/loadEditCurrDestForm/:destId', function (context) {
        const { destId } = context.params;

        DB.collection('allDestinations')
            .doc(destId)
            .get()
            .then(response => {
                context.currDest = { id: destId, ...response.data() };

                extendContext(context)
                    .then(function (context) {
                        this.partial('/templates/editDestinationForm.hbs');
                    })
            })
            .catch(handleError)
    });
    // - send edited destination to DB -
    this.post('/sendEditedDest/:destId', function (context) {
        const { destId, destination, city, duration, departureDate, imgUrl } = context.params;

        const currDestNewData = {
            destination,
            city,
            duration,
            departureDate,
            imgUrl,
            creator: getUserDataFromLocaleStorage().uid,
        };

        DB.collection('allDestinations')
            .doc(destId)
            .set(currDestNewData)
            .then(response => {
                this.redirect(`/loadCurrDestDetails/${destId}`);
            })
            .catch(handleError)
    });

    // - load all my Dest -
    this.get('/loadAllDestinationsFromDB', function (context) {
        const { email, uid } = getUserDataFromLocaleStorage();

        DB.collection('allDestinations')
            .where('creator', '==', uid)
            .get()
            .then(response => {
                context.allDestinations = response.docs.map(currDest => {
                    //console.log(currDest.data())
                    return { id: currDest.id, ...currDest.data() } //currDest.id -> comes from DB, this is  the documentNumber-someID, created by firebase when the movie is created/added into the DB
                });

                //console.log(context.allDestinations)
                
                //context.allDestinations = { ...response.data() }; //ERROR: .data is not a function

                extendContext(context)
                    .then(function (context) {
                        this.partial('/templates/myDestinations.hbs')
                    })
            })
            .catch(handleError)
    });


    // - del Dest -
    this.get('/del/:destId', function (context) {
        const { destId } = context.params;

        DB.collection('allDestinations')
            .doc(destId)
            .delete()
            .then(() => {
                this.redirect('/loadAllDestinationsFromDB')
            })
            .catch(handleError);
    });
});

app.run('/home');

// ----- common functions -----
function extendContext(context) {
    const result = context.loadPartials({
        header: '/templates/partials/header.hbs',
        footer: '/templates/partials/footer.hbs',
    });

    // add to context .isLoggedIn property and assign to it userData from localeStorage
    context.isLoggedIn = Boolean(getUserDataFromLocaleStorage());

    if (context.isLoggedIn) {
        const { email } = getUserDataFromLocaleStorage();
        // add to context .emailOfTheLoggedUser property and assign to it email of the loggedIn user
        context.emailOfTheLoggedUser = email;
    };

    return result;
};

function handleError(err) {
    console.error(err);
};

function saveUserDataInLocaleStorage(userData) {
    const { user: { email, uid } } = userData;
    localStorage.setItem('user', JSON.stringify({ email, uid }));
};

function getUserDataFromLocaleStorage() {
    const user = localStorage.getItem('user');
    const result = JSON.parse(user);
    return result;
};

function removeUserDataFromLocaleStorage() {
    localStorage.removeItem('user');
};


// --- Notifications ---
// function successActionNote(msg) {
//     const elmt = document.getElementById('successBox');

//     elmt.textContent = msg;
//     elmt.parentElement.style.display = 'block';

//     setTimeout(function() {
//         elmt.parentElement.style.display = 'none';
//     }, 5000);
// };

// function failedActionNote(msg) {
//     const elmt = document.getElementById('errorBox');

//     elmt.textContent = msg;
//     elmt.parentElement.style.display = 'block';

//     setTimeout(function() {
//         elmt.parentElement.style.display = 'none';
//     }, 5000);
// };