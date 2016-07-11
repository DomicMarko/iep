﻿window.onload = function () {
    var loadTime = window.performance.timing.domContentLoadedEventEnd - window.performance.timing.navigationStart;
    loadTime = loadTime / 1000;

    var startCalc = -1, endCalc = -1;

    $(function () {

        $('.time-remaining').each(function (i, obj) {

            var fullIDTemp = this.id;
            var resTemp = fullIDTemp.split("_");
            var onlyAuctionIDTemp = resTemp[0];

            var statusIDTemp = onlyAuctionIDTemp + "_status";
            var statusTemp = document.getElementById(statusIDTemp).innerHTML;

            if (statusTemp == 'OPEN') startTimer(this, true);
            else startTimer(this, false);

        });

        function Countdown(options) {            

            var timer,
            instance = this,
            seconds = options.seconds || 10,
            updateStatus = options.onUpdateStatus || function () { },
            counterEnd = options.onCounterEnd || function () { };

            function decrementCounter() {
                updateStatus(seconds);
                if (seconds === 0) {
                    counterEnd();
                    instance.stop();
                }
                seconds--;
            }

            this.start = function () {
                clearInterval(timer);
                timer = 0;
                seconds = options.seconds;
                timer = setInterval(decrementCounter, 1000);
            };

            this.stop = function () {
                clearInterval(timer);
            };
        }

        function startTimer(certainObject, ifLoadTime) {

            var elem = certainObject;

            var fullID = elem.id;
            var res = fullID.split("_");
            var onlyAuctionID = res[0];

            var statusID = onlyAuctionID + "_status";
            var status = document.getElementById(statusID).innerHTML;

            var sec = elem.getAttribute('data-value');
            if (ifLoadTime == true) sec = sec - loadTime;
            sec = parseInt(sec);            

            if (sec > 0) {
                
                if (status == 'OPEN') {

                    var myCounter = new Countdown({
                        seconds: sec, // number of seconds to count down
                        onUpdateStatus: function (sec) {

                            var totalSec = sec;
                            var hours = parseInt(totalSec / 3600) % 24;
                            var minutes = parseInt(totalSec / 60) % 60;
                            var seconds = totalSec % 60;

                            var result = (hours < 10 ? "0" + hours : hours) + ":" + (minutes < 10 ? "0" + minutes : minutes) + ":" + (seconds < 10 ? "0" + seconds : seconds);

                            elem.innerHTML = result;
                        }, // callback for each second
                        onCounterEnd: function () {
                            // Start the connection.
                            $.connection.hub.start().done(function () {

                                stream.server.changeAuctionStatus(onlyAuctionID);
                            });
                        } // final action
                    });

                    myCounter.start();
                }
            }
            else {
                // Start the connection.
                $.connection.hub.start().done(function () {

                    stream.server.changeAuctionStatus(onlyAuctionID);
                });
            }
        }

        // Reference the auto-generated proxy for the hub.
        var stream = $.connection.auctionChanges;

        // Create a function that the hub can call back to display messages.
        stream.client.updateLastBidHome = function (auctionID, fullName, newPrice) {

            var fieldToChangeID = '#' + auctionID + '_lastBid';
            var fieldToChangePriceID = '#' + auctionID + '_lastPrice';

            $(fieldToChangeID).text(fullName);
            $(fieldToChangePriceID).text(newPrice);
        };

        stream.client.changeStartPriceAdmin = function (auctionID, newPrice) {

            var fieldToChangeStartPriceID = '#' + auctionID + '_startPrice';
            var fieldToChangePriceID = '#' + auctionID + '_lastPrice';

            $(fieldToChangeStartPriceID).text(newPrice);
            $(fieldToChangePriceID).text(newPrice);
        };

        stream.client.auctionOpenedHome = function (auctionID, str) {

            var statusID = '#' + auctionID + '_status';

            $(statusID).text('OPEN');

            var timeID = auctionID + "_time";
            var obj = document.getElementById(timeID);

            endCalc = Date.now();
            if (startCalc == -1) startCalc = str;

            loadTime = (endCalc - startCalc) / 1000;
            startTimer(obj, true);
            
            startCalc = -1;            
            endCalc = -1;
        };

        stream.client.auctionStatusChangedOver = function (auctionID) {


        };


        // Start the connection.
        $.connection.hub.start().done(function () {            
            $('.bidBtn').click(function () {
                
                var auctionID = this.getAttribute('data-value');
                var userID = $('#userIDLabel').val();

                // Call the SendBid method on the hub.
                stream.server.sendBid(auctionID, userID);
            });

            $('#changePriceBtn').click(function () {

                var auctionID = this.getAttribute('data-value');
                var newPrice = $('#newStartPriceLabel').val();

                if ((newPrice == null) || (newPrice == ""))
                    alert("Unesite novu početnu cenu aukcije.");
                else
                    stream.server.changeStartPrice(auctionID, newPrice);
            });

            $('#activateAuction').click(function () {
                startCalc = Date.now();
                var auctionID = this.getAttribute('data-value');
                stream.server.activateAuction(auctionID, startCalc);
            });
        });        
    });
}