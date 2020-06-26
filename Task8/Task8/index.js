var clarifaiApiKey = '2e62e2045bf44cfaa2fae0d4db2487c1';
var workflowId = 'my-task8workflow-1';

var app = new Clarifai.App({
  apiKey: clarifaiApiKey
});

// Handles image upload
function uploadImage() {
	var preview = document.querySelector('img');
  var file = document.querySelector('input[type=file]').files[0];
  var reader = new FileReader();

  reader.addEventListener("load", function () {
    preview.src = reader.result;
    var imageData = reader.result;
    imageData = imageData.replace(/^data:image\/(.*);base64,/, '');
    predictFromWorkflow(imageData);
  }, false);

  if (file) {
    reader.readAsDataURL(file);
    preview.style.display = "inherit";
  }
}

// Analyzes image provided with Clarifai's Workflow API
function predictFromWorkflow(photoUrl) {
  app.workflow.predict(workflowId, {base64: photoUrl}).then(
      function(response){
        var outputs = response.results[0].outputs;
        var analysis = $(".analysis");

        analysis.empty();
        console.log(outputs);

        outputs.forEach(function(output) {
          var modelName = getModelName(output);

          // Create heading for each section
          var newModelSection = document.createElement("div");
          newModelSection.className = modelName + " modal-container";

          var newModelHeader = document.createElement("h2");
          newModelHeader.innerHTML = modelName;
          newModelHeader.className = "model-header";

          var formattedString = getFormattedString(output);
          var newModelText = document.createElement("p");
          newModelText.innerHTML = formattedString;
          newModelText.className = "model-text";

          newModelSection.append(newModelHeader);
          newModelSection.append(newModelText);
          analysis.append(newModelSection);
        });
      },
      function(err){
        console.log(err);
      }
  );
}

// Helper function to get model name
function getModelName(output) {
  if (output.model.display_name !== undefined) {
    return output.model.display_name;
  } else if (output.model.name !== undefined) {
    return output.model.name;
  } else {
    return "";
  }
}

// Helper function to get output customized for each model
function getFormattedString(output) {
  var formattedString = "";
  var data = output.data;
  var maxItems = 3;
  // Face Detection
  if (output.model.model_version.id === "f317ccaf2b3343938112179a18293946") {
    var numFaces = data.regions.length;
    if (numFaces === 1) {
      formattedString = "There is 1 face detected in this picture.";
    } else {
      formattedString = "There are " + numFaces + " faces detected in this picture.";
    }
  }
  // Food
  else if (output.model.model_version.id === "dfebc169854e429086aceb8368662641") {
    var items = data.concepts;
    if (items.length < maxItems) {
      maxItems = items.length;
      if (maxItems === 1) {
        formattedString = "The " + maxItems + " food item we are most confident in detecting are:";
      }
    } else {
      formattedString = "The " + maxItems + " food items we are most confident in detecting are:";
    }

    for (var i = 0; i < maxItems; i++) {
      formattedString += "<br/>- " + items[i].name + " at a " + (Math.round(items[i].value * 10000) / 100) + "% probability";
    }
  }
  // NSFW
  else if (output.model.model_version.id === "a6b3a307361c4a00a465e962f721fc58") {
    var items = data.concepts;
    formattedString = "This photo is:";
    for (var i = 0; i < items.length; i++) {
      formattedString += "<br/>- " + items[i].name + " at a " + (Math.round(items[i].value * 10000) / 100) + "% probability";
    }
  }

  return formattedString;
}