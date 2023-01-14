const RATIO = 16/9 // w/h

function resizeHandler(){
  var sHeight = window.innerHeight
  var sWidth = window.innerWidth
  
  // width goes first  
  var calcHeight = sWidth / RATIO
  if(calcHeight <= sHeight){
    canvas.style.width = sWidth+"px"
    canvas.style.height = calcHeight+"px"
    return
  }

  var calcWidth = sHeight * RATIO
  canvas.style.width = calcWidth+"px"
  canvas.style.height = sHeight+"px"
}

window.addEventListener("load", resizeHandler);
window.addEventListener("resize", resizeHandler);
