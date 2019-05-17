thatActive = function (_this, name) {
    var that = document.querySelector("input[name='" + name + "']");
    if (that) {
        that.value = _this.checked;
    }
}

toggleID = function (_this, id) {
    var that = document.querySelector("input[id='" + id + "']");
    if (that) {
        that.value = _this.checked ? id : "";
    }
}
