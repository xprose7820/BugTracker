// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//window.addEventListener('load', function () {
//    let historyStack = JSON.parse(localStorage.getItem('historyStack')) || [];
//    historyStack.push(location.href);
//    localStorage.setItem('historyStack', JSON.stringify(historyStack));
//    history.pushState(null, null, location.href);
//    let specificUrl = "http://localhost:5139/";

//    if (window.location.href === specificUrl && !localStorage.getItem('justLoggedOut')) {
//        console.log("The specific URL was found in the history stack.");
//        fetch('/Login/SignOut', { method: 'GET' })
//            .then(response => {
//                if (!response.ok) {
//                    throw new Error('Network response was not ok');
//                }

//                return response.text();
//            })
//            .then(data => {
//                localStorage.setItem('justLoggedOut', true);
//                location.reload();
//                // process the response if you want
//            })
//            .catch(error => {
//                console.error('There has been a problem with your fetch operation:', error);
//            });
//        //location.reload()

//        // Do something here...
//    } else {
//        localStorage.removeItem('justLoggedOut');
//    }
//});

//window.addEventListener('popstate', function () {
//    let historyStack = JSON.parse(localStorage.getItem('historyStack')) || [];

//    if (historyStack.length > 1) {
//        historyStack.pop();
//        localStorage.setItem('historyStack', JSON.stringify(historyStack));
//    }

//    if (historyStack[historyStack.length - 1] !== location.href) {
//        historyStack.push(location.href);
//        localStorage.setItem('historyStack', JSON.stringify(historyStack));
//        history.pushState(null, null, location.href);
//    } else {
//        console.log("Navigated to a page in the history stack.");
//    }

//    // Check the history stack for a specific URL

//});

//// You can then access the history stack like this:
//console.log(JSON.parse(localStorage.getItem('historyStack')));

window.addEventListener('load', function () {
    let historyStack = JSON.parse(sessionStorage.getItem('historyStack')) || [];
    if (!historyStack.includes(location.href)) {
        historyStack.push(location.href);
    }
    sessionStorage.setItem('historyStack', JSON.stringify(historyStack));
    history.pushState(null, null, location.href);

    let specificUrl = "https://bugtrackerdb.azurewebsites.net/";
    if (window.location.href === specificUrl && !sessionStorage.getItem('justLoggedOut')) {
        console.log("The specific URL was found in the history stack.");
        fetch('/Login/SignOut', { method: 'GET' })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(data => {
                sessionStorage.clear();
                sessionStorage.setItem('justLoggedOut', true);

                location.reload();

            })
            .catch(error => {
                console.error('There has been a problem with your fetch operation:', error);
            });
    } else {
        sessionStorage.removeItem('justLoggedOut');
    }
});

window.addEventListener('popstate', function () {
    let historyStack = JSON.parse(sessionStorage.getItem('historyStack')) || [];

    let currentIndex = historyStack.lastIndexOf(location.href);
    if (currentIndex !== -1) {
        historyStack = historyStack.slice(0, currentIndex + 1);
    } else {
        historyStack.push(location.href);
    }

    sessionStorage.setItem('historyStack', JSON.stringify(historyStack));
    history.pushState(null, null, location.href);
});
console.log(JSON.parse(sessionStorage.getItem('historyStack')));




//////////////// sidebar javascript methods


console.log(sessionStorage.getItem('dashAppearsOnce'));

if (!sessionStorage.getItem('dashAppearsOnce')) {
    sessionStorage.setItem("dashAppearsOnce", 1);
}

console.log(sessionStorage.getItem('dashAppearsOnce'));
if (sessionStorage.getItem('dashAppearsOnce') == 1) {
    const dashLi = document.getElementById("dashboardLi");
    dashLi.classList.add("toggleHoverCSS");
    const aElement = dashLi.querySelector('a');
    aElement.classList.add('text-white');
    sessionStorage.setItem("dashAppearsOnce", 0);
    console.log(sessionStorage.getItem('dashAppearsOnce'));


}
//let counterDashboardAppearOnce = 1;
//if (counterDashboardAppearOnce == 1) {
//    const dashLi = document.getElementById("dashboardLi");
//    dashLi.classList.add("toggleHoverCSS");
//    counterDashboardAppearOnce--;
//}
//if (counterDashboardAppearOnce == 0) {
//    const dashLi = document.getElementById("dashboardLi");
//    dashLi.classList.remove("toggleHoverCSS");
//}


function toggleHover(element) {




    // Store the clicked element's id in session storage
    sessionStorage.setItem('activeNavItem', element.id);
}

window.onload = function () {
    const activeNavItem = sessionStorage.getItem('activeNavItem');


    if (activeNavItem) {

        const activeElement = document.getElementById(activeNavItem);
        if (activeElement) {
            activeElement.classList.add('toggleHoverCSS');
            const aElement = activeElement.querySelector('a');
            aElement.classList.add('text-white');
        }
    }
    
}

//document.addEventListener('click', function (event) {
//    const activeNavItem = sessionStorage.getItem('activeNavItem');
//    if (!(event.target.id == 'dashboardLi' || event.target.id == 'roleLi' || event.target.id == 'personnelLi' || event.target.id == 'projectLi' || event.target.id == 'ticketLi')) {
//        activeElement.classList.remove('toggleHoverCSS');

//    }

//});






