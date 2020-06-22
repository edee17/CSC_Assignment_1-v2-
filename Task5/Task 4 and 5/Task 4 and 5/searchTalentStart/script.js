$('#search').keyup(function () {
    //get data from json file
    //var urlForJson = "data.json";

    //get data from Restful web Service in development environment
    var urlForJson = "/api/talents";

    //get data from Restful web Service in production environment
    //var urlForJson= "http://csc123.azurewebsites.net/api/talents";
    //var bio;
    //var name;
    //var output = '<ul class="searchresults">';
    //Url for the Cloud image hosting
    //var urlForCloudImage = "http://res.cloudinary.com/doh5kivfn/image/upload/v1460006156/talents/";
    //var urlForCloudImage = "https://sp-p1851830-s3-bucket-csc.s3.amazonaws.com/"
    var urlForCloudImage = "https://csc-talentsearch.s3.amazonaws.com/"

    var searchField = $('#search').val();
    var myExp = new RegExp(searchField, "i");
    $.getJSON(urlForJson, function (data) {
        var output = '<ul class="searchresults">';
        $.each(data, function (key, val) {
            //for debug
            console.log(data);
            if ((val.Name.search(myExp) != -1) ||
                (val.Bio.search(myExp) != -1)) {
                output += '<li>';
                output += '<h2>' + val.Name + '</h2>';
                //output += '<img src="images/' + val.ShortName + '_tn.jpg" alt="' + val.Name + '" />';
                //output += '<img src=' + urlForCloudImage + val.ShortName + "_tn.jpg alt=" + val.Name + '" />';
                //output += '<p>' + val.Bio + '</p>';

                //get the image from cloud hosting
                output += '<img src=' + urlForCloudImage + val.ShortName + "_tn.jpg alt=" + val.Name + '" />';
                output += '<p>' + val.Bio + '</p>';
                output += '</li>';

                /*Long URL must start with a protocol, bitly can't (and won't) figure out the right protocol.*/
                ShortLinkBitly(urlForCloudImage + val.ShortName + '_tn.jpg');
            }
        });
        output += '</ul>';
        $('#update').html(output);
    }); //get JSON
});

//var shortURL = ShortLinkBitly("https://sp-p1851830-s3-bucket-csc.s3.amazonaws.com/Barot_Bellingham_tn.jpg");
/*********************************************
* Short Link using Bitly
*********************************************/
function ShortLinkBitly(pLongUrl) { /*pLongUrl is the long URL*/

    /*Long URL must start with a protocol, bitly can't (and won't) figure out the right protocol.*/
    if (!pLongUrl.match(/(ftp|http|https):\/\//i)) {
        return "Error: Link must start with a protocol (e.g.: http or https).";
    }

    var apiKey = 'edde47d00d69ad41f02bf97ac23cbe7b7d9a23a5';
    var username = 'edee17';
    //var test = {
    //    long_url: pLongUrl
    //    };
   
    

    /*Ajax call*/
    $.ajax({
            url: 'https://api-ssl.bitly.com/v3/shorten?login=' + username + '&access_token=' + apiKey + '&format=json&longUrl=' + encodeURIComponent(pLongUrl),
            //  url: 'https://api-ssl.bitly.com/v4/shorten=',
            //dataType: 'jsonp',
            type : 'POST',
            //headers: {
            //    "Authorization":
            //        "Bearer d9bb5dd43b6db46a07f4ab1a293421fbea784482",
            //}, 
            //data: JSON.stringify(test),

            success: function(response) {
              
                console.log(response);
                console.log(response.data.url);
                if (response.status_code == 500) {

                    /*500 status code means the link is malformed.*/
                    return "Error: Invalid link.";

                } else if (response.status_code != 200) {

                    /*If response is not 200 then an error ocurred. It can be a network issue or bitly is down.*/
                    return "Error: Service unavailable.";

                    /*Uncomment the following line only for debugging purposes*/
                    /*console.log('Response: ' + response.status_code + '-' + response.status_txt);*/
                }
                else
                    return response.data.url; /* OK, return the short link */
            },
            error: function (response) {
                console.log(response);
            },

            contentType: 'application/json'
        });

}
/* END: Short Link using Bitly */
