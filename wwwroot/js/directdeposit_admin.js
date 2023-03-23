"use strict"

$(document).ready(function () {
    
    // initialize datatables if it iexists and add the custom toolbar
    function doDataTables() {
        if ($('#ddtable').length > 0) {
            $('#ddtable').DataTable({
                dom: 'l<"toolbar">frtip',
                aoColumnDefs: [
                    { aTargets: [0], bSortable: false },
                    ],
            })
        };
    }

    //=================   DELETE CONFIRMATION   ======================
    $("#btn-delete-user").on("click", function (e) {
        // are you sure you??
        if (confirm("This will permanently remove the user.  Are you sure you want to do this. It cannot be undone.")) {
            return;
        }
        e.preventDefault();
        alert("The user was not deleted.");
    })

    // ============   VALIDATE  EDIT USER FORM
    // ----- reset user password
    $("#resetUserPassword").on("click", function () {

        if ($(this).is(":checked")) {
            $(".reset-password").removeClass("hide");
        } else {
            $(".reset-password").addClass("hide");
        }
    })  

    $('#ddrequest-type').on('change', function (e) {
        let dt = $('#ddtable').DataTable();
        window.location = "?viewtype=" + $(this).val();
    });

    // =========== 
    // check(or uncheck) all boxes and enable / disable action buttons
    $('#select-all-checkboxes').on('change', function (e) {
        //select all checkboxes
        if ($(this).is(":checked")) {
            $(".item-checkbox").prop('checked', true);
            lockButtons(false);
        } else {
            $(".item-checkbox").prop('checked', false);
            lockButtons(true);
        }
        // call update date values
        $('.item-checkbox').each(function (e) {
            //update each date value
            updateDateValues($(this));
        });        
    })

    // select an individual item and enable/disable action buttons
    $('.item-checkbox').on('change', function (e) {
        if ($('.item-checkbox:checked').length > 0) {
            lockButtons(false);
        } else {
            lockButtons(true);
        }
        updateDateValues($(this));
    })

    //function that locks or unlocks action buttons
    function lockButtons(lock) {
        if (lock) {
            //lock the button
            $('div.toolbar .btn-print').prop('disabled', true);
            $('div.toolbar .btn-download').prop('disabled', true);
            $('div.toolbar .btn-completed').prop('disabled', true);
        } else {
            $('div.toolbar .btn-print').prop('disabled', false);
            $('div.toolbar .btn-download').prop('disabled', false);
            $('div.toolbar .btn-completed').prop('disabled', false);
        }
    }

    // function to add or remove completed date from items 
    function updateDateValues(item) {
        // make sure it's not a completed date
        if (item.hasClass("locked")) {

        } else {
            //get current date time, convert to sql format and remove comma (,)
            var currDate = new Date();
            var tmpDate = (currDate.toLocaleString('en-US', { month: '2-digit', day: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', second: '2-digit' })).split(',');
            currDate = tmpDate[0] + tmpDate[1];

            // update the current row's date value
            var id = parseInt(item.val());
            if (item.prop('checked') == true) {
                $('[name="ddInfo[' + id + '].dateProcessed').val(currDate);
            } else {
                $('[name="ddInfo[' + id + '].dateProcessed').val('');
            };
        }
    }


    //print and download buttons
    $('.direct-deposit-requests-container').on('click', '.btn-print', function (e){
        getItems();
    })

    // download selected items
    $('.direct-deposit-requests-container').on('click', '.btn-download', function (e) {
        getItems();
    })

    // mark selected items as completed
    $('.direct-deposit-requests-container').on('click', '.btn-completed', function (e) {
        //get total items to download
        let totalItems = $('.item-checkbox:checked').length;
        if (confirm("You are about to mark " + totalItems + " items as completed. Do you want to contine?")) {
//            getItems();
        } else {
            alert("No action was taken.");
            e.preventDefault();
            return false;
        }
    })

    //loop through selected items
    function getItems() {
        //clear out previous items
        $('.dd-print-container').html('');
        $('.item-checkbox:checked').each(function (e) {
            let pageInfo = [];
            let pageData = '';
            let personData = [];
            //loop through all td's and get data
            $(this).parent().parent().children('td').each(function (ec) {
                personData.push($(this).html());
            })
            // ----- build page
            pageData = '<div class="dd-review-container">';

            // get employee data
            pageData += '<div class="review-group"><h1>Direct Deposit Information for ' + personData[1] + '</h1>';
            pageData += '<table class="user-bank-info"><tr><td>Employee ID:</td><td>' + personData[3] + '</td></tr><tr><td>NetID:</td><td>' + personData[2] + '<td></tr></table>';
            pageData += '<div class="review-group "><h2>' + personData[4] +' Bank Information </h2>';

            // Bank Account Info
            pageData += '<table class="user-bank-info"><tbody>';
            pageData += '<tr><td>Name,Location:</td><td>' + personData[7] + ', ' + personData[8] + '</td></tr><tr><td>Routing Number:</td><td>' + personData[5] + '</td></tr><tr><td>Account Number:</td><td>' + personData[6] + '</td></tr><tr><td>Account Type:</td><td>' + personData[9] + '</td></tr>';
            //check for secondary funds
            if (personData[10].length > 0) {
                pageData += '<tr><td>Dollar Amount:</td><td>' + personData[10] + '</td>';
            }
            // check for image
            if (personData[11].length > 0) {
                pageData += '<tr><td>Check image:</td><tr><td colspan="2"> '+ personData[11] +'</td></tr>';
            }
            pageData += '</tbody></table>';

            // Push data to html container
            pageInfo.push(pageData);
            $('.dd-print-container').append(pageData);
        })

        alert('Data ready.');

        // sytles for pop up box
        let popStyles = "<style>.dd-review-container{page-break-after: always;break-after:page;border-bottom: 1px solid #ccc;}.user-bank-info td{padding: 5px 4px 6px 0;}.user-bank-info{ width: 90%; max-width:90%;}.user-bank-info img {min-height: 200px;max-width: 90%;max-height: 400px;}</style>";

        // create popup window
        let previewWindow = window.open('', 'Direct Deposit Selected Items', 'height = 600, width=800');
        previewWindow.document.write('<html><head><title>Direct Deposit Selected Items</title>');
        previewWindow.document.write(popStyles+'</head><body>');
        previewWindow.document.write('<div class="dd-print-container">' + $(".dd-print-container").html());
        previewWindow.document.write('</body></html>');
        previewWindow.print();

//        $('.print-hide').hide();
//        $('.dd-info-lightbox-background').show();
    }

    doDataTables();
    //build download toolbar and add to datatable
    let downloadButtons = '<button type="button" class="btn action-buttons btn-print" disabled>Print</button> <button type="button" class="btn action-buttons btn-download" value="Download" disabled>Download</button>  <button type="submit" class="btn action-buttons btn-completed" value="Completed" disabled>Completed</button>';
    $('div.toolbar').html('<span class="label-actions">Actions for selected items:</span>' + downloadButtons);
})