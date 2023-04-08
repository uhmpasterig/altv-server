import * as alt from 'alt-client';

let _internalFrontend = null;
_internalFrontend = new alt.WebView("http://resource/ui/index.html");

const Frontend = {
  currentView: null,
  show(view) {
    /* if(this.currentView == view) {
      // close old view???
      return;
    }; */

    Frontend.focus();
    alt.toggleGameControls(false);
    alt.showCursor(true);
    this.currentView = view;
  },
  emit(event, ...args) {
    _internalFrontend.emit(event, ...args);
  },
  send(event, ...args) {
    _internalFrontend.emit(event, ...args);
  },
  on(event, callback) {
    _internalFrontend.on(event, callback);
  },
  recieve(event, callback) {
    _internalFrontend.on(event, callback);
  },
  unfocus() {
    _internalFrontend.unfocus()
  },
  focus() {
    _internalFrontend.focus()
  },
  onClose(view, callback) {
    console.log(this.currentView)
    _internalFrontend.on("closeAll", () => {
      if (this.currentView !== view) return;
      callback();
    });
  },
  isAnyUiOpen() {
    console.log(this.currentView)
    return this.currentView !== null;
  }
}
function _internalOnClose() {
  alt.showCursor(false);
  alt.toggleGameControls(true);
  Frontend.unfocus();
  console.log("close")
  Frontend.currentView = null;
}

Frontend.on("closeAll", _internalOnClose)


export { Frontend };