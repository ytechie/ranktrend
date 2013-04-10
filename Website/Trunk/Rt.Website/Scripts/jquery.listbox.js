jQuery.fn.getItemByValue = function(value)
{
	return this.children("option[@value='" + value + "']");
}

jQuery.fn.selectedItem = function()
{
	return this.children("option[@selected]");
}

jQuery.fn.getSelectedListValue = function()
{
	return this.selectedItem().attr("value");
}

jQuery.fn.setSelectedListValue = function(value)
{
	if(!value)
		this.get(0).selectedIndex = 0;
	else
		this.children("option[@value='" + value + "']").attr("selected", "selected");
	
	return this;
}

//Input: A list item
//Summary: Deletes the item and selects the next list item.
jQuery.fn.deleteItem = function()
{
	var previousIndex;
	var list;
	
	list = this.parent();
	
	previousIndex = list.selectedIndex();
	
	this.remove();
	list.selectItemAfterDelete(previousIndex);
	return this;
}

jQuery.fn.selectItemAfterDelete = function(previousIndex)
{
	var newIndex;
	var itemCount;

	if(previousIndex > 0)
		newIndex = previousIndex - 1;
	else
		newIndex = 0;
		
	itemCount = this.itemCount();
	if(itemCount == 0)
		newIndex = 0;
		
	this.setSelectedIndex(newIndex);

	return this;
}

jQuery.fn.setSelectedIndex = function(index)
{
	this.get(0).selectedIndex = index;
}

jQuery.fn.selectedIndex = function()
{
	return this.get(0).selectedIndex;
}

jQuery.fn.itemCount = function()
{
	return this.get(0).options.length;
}

jQuery.fn.addListItem = function(value, displayText)
{
	this.append('<option value="' + value + '">' + displayText + '</option>');
}