$(document).ready(
	function(){
		var sidrIsShow = 0;
		
		$(".sidr").hide();
		
		jQuery.fn.slideLeftHide = function( speed, callback ) {  
	        this.animate({  
	            width : "hide",  
	            paddingLeft : "hide",  
	            paddingRight : "hide",  
	            marginLeft : "hide",  
	            marginRight : "hide"  
	        }, speed, callback );  
    	};  
	    jQuery.fn.slideLeftShow = function( speed, callback ) {  
	        this.animate({  
	            width : "show",  
	            paddingLeft : "show",  
	            paddingRight : "show",  
	            marginLeft : "show",  
	            marginRight : "show"  
	        }, speed, callback );  
	    };  
		
		$(".navBody img").click(
			function(){
				if(sidrIsShow == 1){
					$(".sidr").slideLeftHide(200);
					sidrIsShow = 0;
				}
				else{
					$(".sidr").slideLeftShow(200);
					sidrIsShow = 1;
				}
				
			}
		);
		
		
		
			
		
		
	}
);



