function notifyMe(shiftId, taskName) {
    if (!window.Notification) {
        console.log('Browser does not support notifications.');
    } else {
        // check if permission is already granted
        if (Notification.permission === 'granted') {
            // show notification here
            var notify = new Notification(taskName, {
                body: 'Пора выполнить задание',
                icon: 'https://www.mtbank.by/upload/images/services/Logo_Product_services_MB_300x300.png',
            });
            notify.addEventListener('click', () => {

                window.open('/Task/ListTask/?shiftId=' + shiftId);
            });
        } else {
            // request permission from user
            Notification.requestPermission().then(function (p) {
                if (p === 'granted') {
                    // show notification here
                    var notify = new Notification(taskName, {
                        body: 'Пора выполнить задание',
                        icon: '/wwwroot/images/Logo_Mtbank.png',
                    });
                    notify.addEventListener('click', () => {

                        window.open('/Task/ListTask/?shiftId=' + shiftId);
                    });
                } else {
                    console.log('User blocked notifications.');
                }
            }).catch(function (err) {
                console.error(err);
            });
        }
    }
}
