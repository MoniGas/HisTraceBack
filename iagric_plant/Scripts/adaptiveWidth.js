(function(){
	var html=document.documentElement;
	var width=html.getBoundingClientRect().width;
	
	html.style.fontSize=width/16+'px';
	//1rem=40px
})()