export default {
  start: () => {
    const script = document.createElement("script");
    script.src = "https://context7.com/widget.js";
    script.dataset.library = "/adsk-duszykf/adsk.platform.toolkit.dotnet";
    script.dataset.color = "#059669";
    script.dataset.position = "bottom-right";
    script.async = true;
    document.body.appendChild(script);
  },
};
