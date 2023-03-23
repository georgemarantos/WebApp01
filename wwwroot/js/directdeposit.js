// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

"use strict"

$(document).ready(function () {

    //new or add selection for manditory primiary account
    $('.newOrAdd').on('click', function (e) {
        if ($(this).val() == "New") {
            //remove locks from others
            $('.no-button').prop("disabled", false);

            //make primary manditory
            $('#select-primary-bank0').prop('checked', true)
            $('#select-primary-bank1').prop("disabled", true);

            //-- display primaryh bank if hidden
            $('.primary-account-group').removeClass('hide');

        } else {
            //remove lock if more than two selected
            if (parseInt($('[name="select-primary-bank"]:checked').val()) + parseInt($('[name="select-secondary-bank"]:checked').val()) + parseInt($('[name="select-travel-bank"]:checked').val()) > 1) {
                $('#select-primary-bank1').prop("disabled", false);
            }
        }
    });

    // ---------------------------- bank account selection toggle
    $('[name="select-primary-bank"]').on("click", function (e) {
        bankSelectToggle($(this), "primary-account-group");
    });

    $('[name="select-secondary-bank"]').on("click", function (e) {
        bankSelectToggle($(this), "secondary-account-group");
    });

    $('[name="select-travel-bank"]').on("click", function (e) {
        bankSelectToggle($(this), "travel-account-group");
    });



    function bankSelectToggle(bankButton, bankGroup) {

        if (bankButton.val() == 1) {
            $("." + bankGroup).removeClass("hide");
            if (parseInt($('[name="select-primary-bank"]:checked').val()) + parseInt($('[name="select-secondary-bank"]:checked').val()) + parseInt($('[name="select-travel-bank"]:checked').val()) > 1) {
                lockNoOptions();
            }
        } else {
            // make sure at least two objects are showing before hiding this one.                     
            if (parseInt($('[name="select-primary-bank"]:checked').val()) + parseInt($('[name="select-secondary-bank"]:checked').val()) + parseInt($('[name="select-travel-bank"]:checked').val()) < 2) {
                //only one item is currently selected - disable the no button (for the currently selected item)
                if ($("#select-primary-bank0").is(":checked")) {
                    $('#select-primary-bank1').prop("disabled", true);
                }
                if ($("#select-secondary-bank0").is(":checked")) {
                    $('#select-secondary-bank1').prop("disabled", true);
                }

                if ($("#select-travel-bank0").is(":checked")) {
                    $('#select-travel-bank1').prop("disabled", true);
                }

            } else {
                lockNoOptions();
            }
            $("." + bankGroup).addClass("hide");

        }
        function lockNoOptions() {
            if ($('.newOrAdd:checked').val() == "New") {
                $(".no-button:not(#select-primary-bank1)").removeClass("button-disabled");
                $(".no-button:not(#select-primary-bank1)").prop("disabled", false);
            } else {
                $(".no-button").removeClass("button-disabled");
                $(".no-button").prop("disabled", false);
            }
        }

    }

    // --------------   Display Lightbox for Review  ------------------ //
    let errorMessage = "";

    function formValidation() {
        //reset error message
        errorMessage = "";

        let validationStatus = true;

        ////validate name information (fn, ln, empid)
        //let eLname = $('[name="User.LastName"]').val();
        //let eFname = $('[name="User.FirstName"]').val();
        //let eNetID = $('#User_NetId').val();
        //let eUid = $('#empID').val();;

        //if (eLname.length == 0 || eFname.length == 0 || eNetID.length ==0 || eUid.length == 0) {
        //    // invalid login information
        //    validationStatus = false;
        //    errorMessage += "<li>Invalid user information</li>";
        //}

        //--------------------validate pay period
        validateDataRadio('User.PayPeriod', 'Please select your current pay period.');

        //------------------validate new or update
        validateDataRadio('newOrAdd', 'Please select if this is your first time (New) or you are making changes (Change).');


        //-----------------validate bank accounts
        // which accounts do we need to validate
        let userBanks = [];
        userBanks.push($('[name="select-primary-bank"]:checked').val());
        userBanks.push($('[name="select-secondary-bank"]:checked').val());
        userBanks.push($('[name="select-travel-bank"]:checked').val());
        let banks = ["primary", "secondary", "travel"];
        let currBank = "";

        for (let i = 0; i < userBanks.length; i++) {
            // validate banks if set to 1
//            alert(banks[i] + ', ' + userBanks[i]);
            if (userBanks[i] == "1") {

                currBank = banks[i];
                // check for blank routing numbers and match
                validateData($('#' + banks[i] + '-routing-number'), 'Please enter the routing number for your ' + banks[i] + ' account.', $('#verify-' + banks[i] + '-routing-number'), 'Please verify the routing number for your ' + banks[i] +' account.',2, "routing numbers")

                // check for matching account numbers
                validateData($('#' + banks[i] + '-account-number'), 'Please enter the account number for your ' + banks[i] + ' account.', $('#verify-' + banks[i] + '-account-number'), 'Please verify the account number for your ' + banks[i] +' account.', 2, "account numbers")

                // check for bank name
                validateData($('#' + banks[i] + '-bank-name'),'Please enter the name of your '+ banks[i] +' financial institute.');

                // check for bank city/state
                validateData($('#' + banks[i] + '-bank-address'), 'Please enter the city,state for your ' + banks[i] +' financial institute.')

                
                // check for bank type (checking/savings)
                switch (i) {
                    case 0:
                        //primary
                        validateDataRadio('Account.Account', 'Please select either savings or checking account for your primary account.');
                        //validate image upload
                        validateData($('[name="FileModel.ImageFileP"]'), 'Please upload a picture of your primary bank routing and account number.');
                        break;
                    case 1:
                        // secondary
                        validateDataRadio('SecondaryAccount.Account', 'Please select either savings or checking account for your secondary account.');
                        //check for bank amount
                        validateData($('#SecondaryAccount_DollarAmount'), 'Please enter the dollar amount for your ' + banks[i] + ' account.');
                        //validate image upload
                        validateData($('[name="FileModel.ImageFileS"]'), 'Please upload a picture of your secondary bank routing and account number.');
                        break;
                    case 2:
                        // travel
                        validateDataRadio('TravelAccount.Account', 'Please select either savings or checking account for your travel account.');
                        //validate image upload
                        validateData($('[name="FileModel.ImageFileT"]'), 'Please upload a picture of your travel bank routing and account number.');

                        break;
                }

                // check for bank amount (if secondary)
                if (banks[i] == 'secondary') {
                    
                }

                //// check for image (ref: )
            }
           
        }
        //------- end bank validation

        //validate string function
        ///  (field#1, error mesage to display, field#2, errormessage to display, 2 // check both fields)
        function validateData(formValue1, errorMsg1, formValue2, errorMsg2, checkBoth = 0, checkWhat) {

            if (formValue1.val().length == 0) {
                //blank account
                validationStatus = false;
                formValue1.parent().addClass('validation-error');
                errorMessage += '<li>' + errorMsg1 + '</li>';
            } else {
                formValue1.parent().removeClass('validation-error');
            }
            if (checkBoth != 0) {
                if (formValue2.val().length == 0) {
                    //blank account
                    validationStatus = false;
                    formValue2.parent().addClass('validation-error');
                    errorMessage += '<li>' + errorMsg2 + '</li>';
                } else {
                    formValue2.parent().removeClass('validation-error');
                    // check for matching numbers
                    if (formValue1.val() != formValue2.val()) {
                        //invalid routing match
                        validationStatus = false;
                        formValue1.parent().addClass('validation-error');
                        formValue2.parent().addClass('validation-error');
                        errorMessage += '<li>The '+ checkWhat +' do not match for the '+ currBank +' account.</li>'
                    } else {
                        formValue1.parent().removeClass('validation-error');
                        formValue2.parent().removeClass('validation-error');
                    }
                }
            }
        }                 


        // compare radio buttons
        function validateDataRadio(radio1, errorMsg) {
            if ($('[name = "' + radio1 + '"]:checked').val() == null) {
                validationStatus = false;
                $('[name = "' + radio1 + '"]').parent().parent().addClass('validation-error');
                errorMessage += '<li>' + errorMsg + '</li>';
            } else {
                $('[name = "' + radio1 + '"]').parent().parent().removeClass('validation-error');
            }
        }


        // ============  validate signature
        let empSig = $('.employee-signature')
        if (empSig.val().length < 3) {
            validationStatus = false;
            $(empSig).parent().addClass('validation-error');
            errorMessage += '<li>Please type your full name in the signature box.</li>';
        } else {
            $(empSig).parent().removeClass('validation-error');
        }


        return validationStatus
    }

    $('.review-button').on('click', function (e) {
        e.preventDefault();
        //validate before review....
        if (formValidation() == false) {
            $('#ddform .errorMessages').html('<p class="errors">Please correct the errors listed below:</p> <ul>' + errorMessage + '</ul>');
            alert("Please correct the errors displayed at the top of the page and resubmit the form.");
            //$('#ddform .errorMessages').scrollTop();
            $('#ddform .errorMessages').removeClass('hide');
            document.getElementById("ddform").scrollIntoView();
            return
        } else {
            $('#ddform .errorMessages').addClass('hide');
        }

        // -- name
        $('.review-name').html($('[id = "employeelastname"]').val() + ", " + $('[id = "employeefirstname"]').val() + " " + $('[id = "employeemiddleinitial"]').val());
        $('.review-empid').html($('[name = "employeeid"]').val());

        // -- empid, pay-period-selection
        $('.review-payperiod').html($('[id = "pay-period-selection"]:checked').val());
        // -- new, update bank
        $('.review-update').html($('[name="newOrAdd"]:checked').val());


        // -- primary account //routing#,account#,BankName,CityState,routing#2,account#2,checking/savings,image
        if ($('[name ="select-primary-bank"]:checked').val() == 1) {
            //display primary container
            $('.review-primary').removeClass('hide');
            $('.review-primary-name').html($('[id="primary-bank-name"]').val());
            $('.review-primary-address').html($('[id="primary-bank-address"]').val());
            $('.review-primary-routing-number').html($('[id="primary-routing-number"]').val());
            $('.review-primary-bank-account-number').html($('[id="primary-account-number"]').val());
            $('.review-primary-account-type').html($('[id="Account_Account"]:checked').val());
        } else {
            $('.review-primary').addClass('hide');
        }
        // -- secondary account //routing#,account#,BankName,CityState,routing#2,account#2,checking/savings,image
        if ($('[name="select-secondary-bank"]:checked').val() == 1) {
            $('.review-secondary').removeClass('hide');
            $('.review-secondary-name').html($('[id="secondary-bank-name"]').val());
            $('.review-secondary-address').html($('[id="secondary-bank-address"]').val());
            $('.review-secondary-routing-number').html($('[id="secondary-routing-number"]').val());
            $('.review-secondary-bank-account-number').html($('[id="secondary-account-number"]').val());
            $('.review-secondary-account-type').html($('[id="SecondaryAccount_Account"]:checked').val());
            $('.review-secondary-amount').html($('[id="SecondaryAccount_DollarAmount"]').val());
        } else {
            $('.review-secondary').addClass('hide');
        }

        // -- travel bank account //routing#,account#,BankName,CityState,routing#2,account#2,checking/savings,image
        if ($('[name="select-travel-bank"]:checked').val() == 1) {
            $('.review-travel').removeClass('hide');
            $('.review-travel-name').html($('[id="travel-bank-name"]').val());
            $('.review-travel-address').html($('[id="travel-bank-address"]').val());
            $('.review-travel-routing-number').html($('[id="travel-routing-number"]').val());
            $('.review-travel-bank-account-number').html($('[id="travel-account-number"]').val());
            $('.review-travel-account-type').html($('[id="TravelAccount_Account"]:checked').val());
        } else {
            $('.review-travel').addClass('hide');
        }



        $('#ddform fieldset input').prop('disabled', true);
        $('.dd-info-lightbox-background').css('display', 'block');

        // lock form information


        //move to top of page
        $('html,body').scrollTop(0);
    })

    // ---------------   close lightbox (edit information) ---------------//
    $('.edit-button').on('click', function (e) {
        e.preventDefault();
        $('#ddform fieldset input').prop('disabled', false);
        $('.dd-info-lightbox-background').css('display', 'none');
    })

    // ---------------   submit button ----------------------//
    $('.submit-button').on('click', function (e) {
        $('#ddform fieldset input').prop('disabled', false);
//        $('.dd-info-lightbox-background').css('display', 'none');
        $('#ddform .submit-button').val("Submitting Information");
        $('#ddform .edit-button').prop('disabled', true);
        $('#ddform .submitted-info').prop('diabled', true);
        $('#ddform .submitted-info').removeClass('hide');
        $('#ddform .submit-button').hide();

    })

    // -------------------------------------  reset area fields
    $('.resetbutton').on('click', function (e) {
        e.preventDefault();
        $("." + $(this).val() + " input").val("");
        $("." + $(this).val() + " img").attr("src", "");
    })

})