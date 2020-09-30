var static_resource_path = "https://static.eduso.vn/";

function cacheStatic(src) {
    if (src == "") return "";
    if (src.startsWith("http"))
        return src;
    return static_resource_path + src + "?&format=jpg";
}
function cacheStatic(src, width) {
    if (src == "") return "";
    if (src.startsWith("http"))
        return src;
    return static_resource_path + src + "?&format=jpg&w=" + width;
}
function cacheStatic(src, width, height, alt) {
    if (src == null && alt != null) return cacheStatic(alt, width);
    if (src.startsWith("http"))
        return src;
    return static_resource_path + src + "?w=" + width + "&h=" + height + "&mode=crop&format=jpg";
}
