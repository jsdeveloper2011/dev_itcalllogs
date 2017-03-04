app.service("crudWorkLogsService", function ($http) {

    //get All Books
    this.getWorkLogs = function () {
        return $http.get("WorkLogs/GetAllWorkLogs");
    };

    // get Book by bookId
    this.getWorkLogs = function (WorkID) {
        var response = $http({
            method: "post",
            url: "WorkLogs/GetWorkLogsbyId",
            params: {
                id: JSON.stringify(WorkID)
            }
        });
        return response;
    }

    //// Update Book 
    //this.updateBook = function (book) {
    //    var response = $http({
    //        method: "post",
    //        url: "Home/UpdateBook",
    //        data: JSON.stringify(book),
    //        dataType: "json"
    //    });
    //    return response;
    //}

    //// Add Book
    //this.AddBook = function (book) {
    //    var response = $http({
    //        method: "post",
    //        url: "Home/AddBook",
    //        data: JSON.stringify(book),
    //        dataType: "json"
    //    });
    //    return response;
    //}

    ////Delete Book
    //this.DeleteBook = function (bookId) {
    //    var response = $http({
    //        method: "post",
    //        url: "Home/DeleteBook",
    //        params: {
    //            bookId: JSON.stringify(bookId)
    //        }
    //    });
    //    return response;
    //}
});