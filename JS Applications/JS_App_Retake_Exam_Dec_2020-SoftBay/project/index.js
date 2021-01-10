const UserModel = firebase.auth();
const DB = firebase.firestore();

// ----- main logic -----
const app = Sammy('#root', function () {
    this.use('Handlebars', 'hbs');

    // --- load (homePage.hbs) ---
    this.get('/home', function (context) {

        DB.collection('allOffers')
            .get()
            .then(response => {

                context.allOffers = response.docs.map(currOffer => { return { ...currOffer.data(), id: currOffer.id } });
                //console.log(context.currOffer)

                extendContext(context)
                    .then(function () {
                        this.partial('/templates/homePage.hbs')
                    })
            })
            .catch(handleError)
    });

    // --- user rout ---
    // - load registerForm -
    this.get('/loadRegisterForm', function (context) {
        extendContext(context)
            .then(function () {
                this.partial('/templates/registerForm.hbs');
            });
    });
    // - register user -> POST request -
    this.post('/registerUser', function (context) {
        const { email, password, repeatPassword } = context.params;

        if (email === '' || password === '' || repeatPassword === '') {
            throw new Error('Register fields must not be empty!');
            //return;
        };
        if (password !== repeatPassword) {
            throw new Error('Password and Repeat password must be the same!');
            //return;
        };

        UserModel.createUserWithEmailAndPassword(email, password)
            .then(userData => {
                saveUserDataInLocaleStorage(userData);
                this.redirect('/home');
            })
            .catch(handleError);
    });

    // - load loginForm -
    this.get('/loadLoginForm', function (context) {
        extendContext(context)
            .then(function () {
                this.partial('/templates/loginForm.hbs');
            });
    });
    // - login user -> POST request -
    this.post('/loginUser', function (context) {
        const { email, password } = context.params;

        if (email === '') {
            throw new Error('Email field must not be empty!');
            //return;
        };
        if (password.length < 6 || password.length === '') {
            throw new Error('Invalid password!');
            //return;
        };

        UserModel.signInWithEmailAndPassword(email, password)
            .then(userData => {
                saveUserDataInLocaleStorage(userData);
                this.redirect('/home');
            })
            .catch(handleError);

    });
    // - logout user -
    this.get('/logoutUser', function (context) {
        UserModel.signOut()
            .then(() => {
                removeUserDataFromLocaleStorage();
                this.redirect('/home');
            })
            .catch(handleError)
    });

    // --- offers rout ---
    // - load createOfferForm - 
    this.get('/loadCreateOfferForm', function (context) {
        extendContext(context)
            .then(function (context) {
                this.partial('/templates/createOfferForm.hbs');
            });
    });
    // - create/send new offer to DB - POST request - 
    this.post('/createOffer', function (context) {
        const { product, description, price, pictureUrl } = context.params;

        if (product === '' || description === '' || price === '' || pictureUrl === '') {
            throw new Error('Input fields must not be empty!');
            //return;
        };

        const newOffer = {
            product,
            description,
            price,
            pictureUrl,
            creator: getUserDataFromLocaleStorage().email,
        };

        DB.collection('allOffers')
            .add(newOffer)
            .then(response => {
                this.redirect('/loadDashboard') //to dashboard
            })
            .catch(handleError)
    });

    // - load dashboard view - 
    this.get('/loadDashboard', function (context) {
        const { email, uid } = getUserDataFromLocaleStorage();

        DB.collection('allOffers')
            .get()
            .then(response => {
                context.allOffers = response.docs.map(currOffer => {
                    const isCreator = Boolean(email === currOffer.data().creator);
                    //console.log(currOffer.data())
                    return { id: currOffer.id, ...currOffer.data(), isCreator } //currOffer.id -> comes from DB, this is  the documentNumber-someID, created by firebase when the offer is created/added into the DB
                });

                //console.log(context.allOffers)

                extendContext(context)
                    .then(function (context) {
                        this.partial('/templates/dashboard.hbs')
                    })
            })
            .catch(handleError)
    });
    // - load editOfferForm - 
    this.get('/loadEditOfferForm/:offerId', function (context) {
        const { offerId } = context.params;
        //const { offerId, title, description, imageUrl } = context.params;

        DB.collection('allOffers')
            .doc(offerId)
            .get()
            .then(response => {
                context.currOffer = { ...response.data(), id: offerId };

                extendContext(context)
                    .then(function () {
                        this.partial('/templates/editOfferForm.hbs');
                    });
            })
            .catch(handleError)
    });
    // - send edited offer to DB - POST request -
    this.post('/editOffer/:offerId', function (context) {
        const { offerId, product, description, price, pictureUrl } = context.params;

        const newDataCurrOffer = {
            product,
            description,
            price,
            pictureUrl,
            creator: getUserDataFromLocaleStorage().email,
        };

        DB.collection('allOffers')
            .doc(offerId)
            .set(newDataCurrOffer)
            .then(response => {
                this.redirect('/loadDashboard');
                //this.redirect('/home');
                //this.redirect(`/loadEditOfferForm/${offerId}`);
            })
            .catch(handleError);
    });
    // - load delete offer form -
    this.get('/loadDelOfferForm/:offerId', function (context) {
        const { offerId } = context.params;
        //const { movieId, title, description, imageUrl } = context.params;

        DB.collection('allOffers')
            .doc(offerId)
            .get()
            .then(response => {
                context.currOffer = { ...response.data(), id: offerId };

                extendContext(context)
                    .then(function () {
                        this.partial('/templates/deleteOfferForm.hbs');
                    });
            })
            .catch(handleError)
    });
    // - delete curr offer - POST request -
    this.post('/del/:offerId', function (context) {
        const { offerId } = context.params;

        DB.collection('allOffers')
            .doc(offerId)
            .delete()
            .then(() => {
                this.redirect('/home');
            })
            .catch(handleError)
    });

    // - load offer details form - 
    this.get('/loadOfferDetailsForm/:offerId', function (context) {
        const { offerId } = context.params;
        const { email, uid } = getUserDataFromLocaleStorage();

        DB.collection('allOffers')
            .doc(offerId)
            .get()
            .then(response => {
                //const isCreator = Boolean(email === response.data().creator);
                //const hasLikedMovie = Boolean(response.data().likes.find((currEmail) => currEmail === email));

                //context.currMovie = { ...response.data(), id: offerId, isCreator, hasLikedMovie }
                context.currOffer = { ...response.data(), id: offerId }

                extendContext(context)
                    .then(function () {
                        this.partial('/templates/offerDetails.hbs')
                    });
            })
            .catch(handleError);
    });

    // - buy offer -


    // - load profile page view -
    this.get('/loadProfilePage', function (context) {
        extendContext(context)
            .then(function (context) {
                this.partial('/templates/profilePage.hbs');
            });
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