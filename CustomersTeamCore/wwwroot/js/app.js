$(function () {

    // Table headers

    $("#login-hearer").click(function (event) {
        setHeaderSorting(event.target);
        loadCustomers();
        return false;
    });

    $("#name-header").click(function (event) {
        setHeaderSorting(event.target);
        loadCustomers();
        return false;
    });

    $("#mail-header").click(function (event) {
        setHeaderSorting(event.target);
        loadCustomers();
        return false;
    });

    $("#phone-header").click(function (event) {
        setHeaderSorting(event.target);
        loadCustomers();
        return false;
    });

    // Delete and page elements

    $('body').on('click', 'img.delete-button', function (event) {
        if (confirm('Are you sure you want to delete customer?')) {
            deleteCustomer($(event.target));
        }
    });

    $('body').on('click', 'a.page-link', function (event) {
        $(".page-item").each(function (index, element) {
            $(element).removeClass("active");
        });

        $(event.target).parent().addClass("active");
        loadCustomers();

        return false;
    });

    // Search

    $("#search-form").submit(function (e) {
        e.preventDefault();
        loadCustomers();
    });

    // Select All

    $("#select-header").change(function (e) {
        let currentState = this.checked;
        $(".row-checkbox").each(function (index, element) {
            $(element).prop('checked', currentState);
        })
    });

    // Load data

    loadCustomers(); 
});

function setHeaderSorting(sender) {

    if (sender === undefined || sender === null) {
        return;
    }

    let currentSort = $(sender).data("sort");
    if (currentSort == "none") {
        $(sender).data("sort", "asc");
    }
    else {
        $(sender).data("sort", currentSort === "asc" ? "desc" : "asc");
    }

    let hearers = $(".sort-header");
    hearers.each(function (index, element) {
        if ($(element).attr("id") !== $(sender).attr("id")) {
            $(element).data("sort", "none");
        }
    });
}

function deleteCustomer(sender) {
    let customerId = sender.data("id");
    $.ajax({
        url: "/api/customers/delete",
        type: "DELETE",
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        data: JSON.stringify(customerId),
        success: function (data) {
            if (data.success) {
                loadCustomers();
            } else {
                alert(date.message);
            }
        },
        error: function (response) {
            alert("Server error on deletion");
        }
    });
}

function loadCustomers() {
    $.ajax({
        url: "/api/customers/fetch?" + prepareQueryParameters(),
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        success: function (data) {
            fillData(data);
        },
        error: function (response) {
            console.log("Load customers error");
        }
    });
};

function fillData(data) {

    if (data === undefined || data === null) {
        console.log("Wrong data");
        return;
    }

    $("#select-header").prop('checked', false);
    $("#total-count").text(data.totalCustomers);

    fillTable(data.customers);
    fillPagination(data.page, data.totalPages);
}

function fillTable(data) {

    if (data == undefined || data == null) {
        console.log("No customers");
        return;
    }

    $("#customers-table").find('tbody').empty();

    for (let i = 0; i < data.length; i++) {

        if (data[i] === undefined || data[i] === null) {
            console.log("Empty data record. Number: " + i);
            continue;
        }

        $("#customers-table").find('tbody')
            .append($('<tr>')
                .append($('<td>').append(createCheckBox(data[i])))
                .append($('<td>').append(createEditLink(data[i], "login")))
                .append($('<td>').append(createEditLink(data[i], "fullName")))
                .append($('<td>').append(createMailLink(data[i])))
                .append($('<td>').append(data[i].phone))
                .append($('<td>').append(data[i].active == true
                    ? $('<img>').attr('src', '/images/active.png')
                    : $('<img>').attr('src', '/images/unactive.png')))
                .append($('<td>').append(prepareEditElements(data[i])))
            );
    }
};

function fillPagination(current, totalPages) {

    $("#pagination-list").empty();

    for (let i = 1; i <= totalPages; i++) {
        let liClass = (current === i ? "page-item active" : "page-item");
        let page = "<li class='" + liClass + "'><a class='page-link' href='#' data-page='" + i + "'>" + i + "</a></li>";
        $('#pagination-list').append($(page));
    }
};

function prepareEditElements(data) {
    let editLink = "<a href='/Customer/Edit/" + data.id + "'><img style='margin-right: 15px' src='/images/edit.png'></a>";
    let deleteLink = "<img class='delete-button' src='/images/delete.png' style='cursor:pointer' data-id='" + data.id + "'>";

    return editLink + deleteLink;
};

function createEditLink(data, field) {

    let canEdit = $("#can-edit").val();
    if (canEdit == "true") {
        return "<a href='/Customer/Edit/" + data.id + "'>" + data[field] + "</a>";
    } else {
        return data[field];
    }
};

function createMailLink(data) {
    if (data.email === undefined || data.email === null) {
        return "";
    }

    return "<a href='mailto:'" + data.email + "'>" + data.email + "</a>"
};

function createCheckBox(data) {
    let checkbox = $('<input>')
        .attr({
            type: "checkbox",
            id: "check-customer-" + data.id,
            name: "check-customer-" + data.id,
            class: "row-checkbox",
        })
        .data("customer", data.id);

    return checkbox;
}

function prepareQueryParameters() {

    let selectedPage = $("#pagination-list").find("li.active").children("a").data("page");

    let params = {
        loginSort: $("#login-hearer").data("sort"),
        nameSort: $("#name-header").data("sort"),
        emailSort: $("#mail-header").data("sort"),
        phoneSort: $("#phone-header").data("sort"),
        s: $('#search-input').val(),
        page: selectedPage !== undefined && selectedPage != null ? selectedPage : 1,
    };

    return $.param(params);
};