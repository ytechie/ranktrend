/////////////////////
//	These methods are called from the onclick events of the buttons
/////////////////////

function deleteRecord(sourceControl, recordKey)
{
	if(!confirm("Are you sure you want to delete this record?"))
		return false;
	
	deleteRow(sourceControl, recordKey);
	
	//Return false so that the server doesn't process the delete
	return false;
}

function reprocessRecord(sourceControl, recordKey)
{
	//Make a server call to reprocess the record
	ReprocessItem(recordKey, function(result) { reprocessCallback(result, sourceControl); });
	//$.getJSON("Default.aspx", { reprocessId: recordKey }, function(jsonData) { reprocessCallback(jsonData, sourceControl); });

	
	//Return false so that the server doesn't process the delete
	return false;
}

/////////////////////

function reprocessCallback(result, sourceControl)
{
	if(result.toLowerCase() == true.toString())
		animateAndRemoveRow(sourceControl);
	else
		$(sourceControl).parents("tr").css("background-color", "red").css("color", "white");
}

function deleteRow(sourceControl, id)
{
	DeleteQueueItem(id, function(jsonData) { animateAndRemoveRow(sourceControl); });
	//$.get("Default.aspx", { deleteId: id }, function(data) { animateAndRemoveRow(sourceControl); });
}

function animateAndRemoveRow(clickedElement)
{
	$(clickedElement).parents("tr").children("td").fadeOut("slow", function() { rowFadeoutComplete(clickedElement); });
}

function rowFadeoutComplete(clickedElement)
{
	$(clickedElement).parents("tr").hide();
}