customElements.get("stop-watch-msec") ?? customElements.define("stop-watch-msec",
    class StopWatch extends HTMLElement {
        constructor() { super(); }
        static get observedAttributes() { return ["start"]; }
        attributeChangedCallback(name, oldValue, newValue) { this[name] = newValue; }
        get start() { return this._start; }
        set start(value) { this._start = Date.parse(value); }
        connectedCallback() { this.interval = setInterval(() => this.innerText = Date.now() - this.start, 50); }
        disconnectedCallback() { clearInterval(this.interval); }
    }
);