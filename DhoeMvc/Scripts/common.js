//以application/json ContentType傳送JSON字串到Server端
jQuery.postJson = function (url, data, callback, type) {
  if (jQuery.isFunction(data)) {
    type = type || callback;
    callback = data;
    data = undefined;
  }

  return jQuery.ajax({
    url: url,
    type: "POST",
    dataType: type,
    contentType: "application/json",
    data: typeof (data) == "string" ? data : JSON.stringify(data),
    success: callback
  });
};