/*
  * toJson
  *
  * produces a JSON string representation of a javascript object
  * usage: var jsonstring = toJSON(someobject);
  *
  * Tino Zijdel - crisp@xs4all.nl, 28/09/2006
  */
function toJSON(obj)
{
	if (typeof obj == 'undefined')
		return 'undefined';
	else if (obj === null)
		return 'null';

	var stringescape = {
		'\b': '\\b',
		'\t': '\\t',
		'\n': '\\n',
		'\f': '\\f',
		'\r': '\\r',
		'"' : '\\"',
		'\\': '\\\\'
        }

	var json = null, i, l, v, a = [];
	switch (obj.constructor)
	{
		case Array:
			l = obj.length;
			for (i = 0; i < l; i++)
			{
				if ((v = toJSON(obj[i])) !== null)
					a.push(v);
			}
			json = '[' + a.join(',') + ']';
			break;
		case Object:
			for (i in obj)
			{
				if (obj.hasOwnProperty(i) && (v = toJSON(obj[i])) !== null)
					a.push(toJSON(String(i)) + ':' + v);
			}
			json = '{' + a.join(',') + '}';
			break;
		case String:
			json = '"' + obj.replace(/[\x00-\x1f\\"]/g, function($0)
			{
				var c;
				if ((c = stringescape[$0]))
					return c;
				c = $0.charCodeAt();
				return '\\u00' + Math.floor(c / 16).toString(16) + (c % 16).toString(16);
			}) + '"';
			break;
		case Number:
			json = isFinite(obj) ? String(obj) : 'null';
			break;
		case Boolean:
			json = String(obj);
			break;
	}

	return json;
}