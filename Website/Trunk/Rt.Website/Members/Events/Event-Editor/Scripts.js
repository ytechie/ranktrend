//Page scoped control references for performance
var ddlSites;
var ddlEventCategories;

$(documentReady);

function documentReady()
{
	initControlReferences();
	initControlReferences2();
	
	ddlSites.change(ddlSites_changed);
	ddlEventCategories.change(ddlCategories_changed);

	$(document.lstEventsId).change(lstEvents_changed);
	$(document.cmdAddId).click(cmdAdd_clicked);
	$(document.cmdDeleteId).click(cmdDelete_clicked);
	
	$(document.chkHasEndId).click(chkHasEndId_changed);
	$(document.txtStartId).calendar({dateFormat: 'MDY/'});
	$(document.txtEndId).calendar({dateFormat: 'MDY/'});
	$(document.cmdSaveId).click(cmdSave_click);
	
	selectFirstEvent();
}

function initControlReferences2()
{
	ddlSites = $(document.ddlSitesId);
	ddlEventCategories = $(document.ddlCategoriesId);
}

function lstEvents_changed()
{
	displaySelectedEvent();
}

function cmdAdd_clicked()
{
	var selectedSiteId;
	var selectedCategoryId;
	
	selectedSite = ddlSites.getSelectedListValue();
	selectedCategoryId = ddlEventCategories.getSelectedListValue();

	AddEvent(selectedSite, selectedCategoryId, cmdAdd_callback);
	
	//Don't post back to the server
	return false;
}

function cmdAdd_callback(eventData)
{
	addEvent(eventData);
}

function cmdDelete_clicked()
{
	var selectedItem;
	var eventData;
	
	selectedItem = $(document.lstEventsId).selectedItem();
	eventData = getDisplayedEventData();
	
	if(eventData != null)
		DeleteEvent(parseInt(eventData.Id), function(){ itemDeleteComplete(selectedItem); });
		
	//Don't post back to the server
	return false;
}

function itemDeleteComplete(selectedItem)
{
	//Once the item is deleted, remove it from the listbox
	selectedItem.deleteItem();
	
	displaySelectedEvent();
}

//If there is at least one event in the list, it
//gets selected, and its details are displayed.
function selectFirstEvent()
{
	if($(document.lstEventsId).itemCount() > 0)
		$(document.lstEventsId).setSelectedIndex(0);

	displaySelectedEvent();
}

function ddlSites_changed()
{
	refreshEventCategoryList();
}

function refreshEventCategoryList()
{
	var siteId;
	
	siteId = ddlSites.getSelectedListValue();
	
	//Get the categories from the server
	GetEventCategoryList(siteId, GetEventCategoryList_callback);
}

function GetEventCategoryList_callback(categories)
{
	//Clear the previous category list
	ddlEventCategories.empty();
	
	//Add the "Unassigned" item
	ddlEventCategories.addListItem("Any", "&lt;Default Category&gt;");
	
	//Add the new category list
	for(var i = 0; i < categories.length; i++)
		ddlEventCategories.addListItem(categories[i].Value, categories[i].Text);
		
	//The first item in the list should be selected
	ddlEventCategories.setSelectedIndex(0);
	
	refreshEventList();
}

//Fired when the category criteria changes
function ddlCategories_changed()
{
	refreshEventList();
}

function refreshEventList()
{
	var categoryId;
	var siteIdString;
	
	siteIdString = ddlSites.getSelectedListValue();
	categoryIdString = ddlEventCategories.getSelectedListValue();
	
	if(!siteIdString || !categoryIdString)
	{
	//todo: disable some stuff
		return;
	}
	
	GetEventList(siteIdString, categoryIdString, eventListReceived);
}

function eventListReceived(eventList)
{
	var lstEvents;
	
	lstEvents = $(document.lstEventsId);
	lstEvents.empty(); //clear the event list
	
	for(var i = 0; i < eventList.length; i++)
		addEvent(eventList[i]);
	
	selectFirstEvent();
}

function addEvent(eventData)
{
	var lstEvents;
	
	lstEvents = $(document.lstEventsId);
	lstEvents.addListItem(eventData.Id, eventData.Name);
	newOption = lstEvents.children(":last-child");
	setEventData(newOption, eventData);
	
	displaySelectedEvent();
}

function displaySelectedEvent()
{
	var eventData;
	
	eventData = getDisplayedEventData();
	
	if(eventData == null)
		displayEvent(null);
	else
		displayEvent(eventData);
}

function getDisplayedEventData()
{
	return getEventData($(document.lstEventsId).selectedItem());
}

function getEventData(listItem)
{
	var eventData;
	var eventJSON;
	
	eventData = listItem.attr("event");
	if(eventData != null)
		return eventData;
	
	eventJSON = listItem.attr("eventJSON");
	return eval("(" + eventJSON + ")");
}

function setEventData(listItem, eventData)
{
	listItem.attr("value", eventData.Id);
	listItem.attr("event", eventData);
	listItem.text(eventData.Name);
}

function displayEvent(eventData)
{
	if(eventData == null)
	{
		$(document.txtNameId).attr("value", "").attr("disabled", "disabled");
		$(document.txtDescriptionId).attr("value", "").attr("disabled", "disabled");
		$(document.txtStartId).attr("value", "").attr("disabled", "disabled");
		$(document.txtEndId).attr("value", "").attr("disabled", "disabled");
		$(document.chkHasEndId).attr("value", "").attr("disabled", "disabled");
		$(document.ddlColorId).attr("disabled", "disabled");
	}
	else
	{
		$(document.txtNameId).removeAttr("disabled").attr("value", eventData.Name);
		$(document.txtDescriptionId).removeAttr("disabled").attr("value", eventData.Description ? eventData.Description : "");
		$(document.txtStartId).removeAttr("disabled").attr("value", eventData.StartString);
		if(eventData.EndString == null)
		{
			$(document.txtEndId).attr("disabled", "disabled").attr("value", "");
			$(document.chkHasEndId).removeAttr("disabled").removeAttr("checked");
		}
		else
		{
			$(document.txtEndId).removeAttr("disabled").attr("value", eventData.EndString);
			$(document.chkHasEndId).removeAttr("disabled").attr("checked", "checked");
		}

		resetItemCategory();

		if(eventData.ColorName == "0")
			$(document.ddlColorId).setSelectedListValue(null).removeAttr("disabled");
		else
			$(document.ddlColorId).setSelectedListValue(eventData.ColorName).removeAttr("disabled");
	}
}

//Changes the category for the displayed item back to
//the category filter being dispayed.
function resetItemCategory()
{
	var selectedCategory;
	
	selectedCategory = ddlEventCategories.getSelectedListValue();
	$(document.ddlDetailCategoriesId).setSelectedListValue(selectedCategory);
}

//When the user unchecks this, disable the end date entry
function chkHasEndId_changed()
{
	if($(document.chkHasEndId).attr("checked") != null)
		$(document.txtEndId).removeAttr("disabled");
	else
		$(document.txtEndId).attr("disabled", "disabled");
}

function cmdSave_click()
{
	var eventDetails;
	var jsonString;
	
	//Change the visual style of the save button so they know a save is happening
	$(document.cmdSaveId).attr("value", "Saving...").attr("disabled", "disabled");
	
	eventDetails = getDisplayedEventDetails();
	
	jsonString = toJSON(eventDetails);
	//verify the jsonString is valid
	Save(jsonString, function() { saveCallback(eventDetails); });
	
	//Don't postback
	return false;
}

function saveCallback(eventDetails)
{
	var selectedEventListItem;
	
	//Give the user some eye candy and a way to know the save was a success
	showSavedMessage();
	
	selectedEventListItem = $(document.lstEventsId).selectedItem();
	setEventData(selectedEventListItem, eventDetails);
	
	//TO DO: determine if they changed the event category
	
	//Re-enable the save button
	$(document.cmdSaveId).attr("value", "Save").removeAttr("disabled");
}

function showSavedMessage()
{
	$("#saveSuccessMsg").stop().show().fadeOut(2000);
}

function getDisplayedEventDetails()
{
	var json;
	
	json = new Object();
	
	json.Id = $(document.lstEventsId).getSelectedListValue();
	json.Name = $(document.txtNameId).attr("value");
	json.Description = $(document.txtDescriptionId).attr("value");
	json.StartString = $(document.txtStartId).attr("value");
	if($(document.chkHasEndId).attr("checked") != null)
		json.EndString = $(document.txtEndId).attr("value");
	else
		json.EndString = null;
	json.EventCategoryId = $(document.ddlDetailCategoriesId).getSelectedListValue();
	json.ColorName = $(document.ddlColorId).getSelectedListValue();
	
	return json;
}