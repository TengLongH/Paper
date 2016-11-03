$(document).ready(
	
	
	function(){
		
		
		var loginApp = angular.module('ngLoginApp',[]);
		loginApp.controller('ngLoginCtrl',function($scope, $http){
			
						
					
					
					
					
					$scope.login = function(){
						$http.post("http://localhost:61355/api/login",$scope.loginData)
						.success(
							function(response){
								
								if(response == "success"){
									if($scope.loginData.role == "common"){
										window.location.href = "Views/User/UserMain.html";
									}
									else if($scope.loginData.role == "depart"){
										window.location.href = "Views/Admin/AdminMain.html";
									}
									else if($scope.loginData.role == "school"){
										alert("功能暂未开放！攻城狮正在努力建设中……");
									}
								}
								else if(response == "noUser"){
									alert("没有此用户喵！请检查您的用户名或身份是否正确喵~");
									window.location.reload();
								}
								else if(response == "wrongPassword"){
									alert("用户名或密码可能输错了喵~再来一遍喵~");
									window.location.reload();
								}
								
							}
						).error(
							
							function(){
								alert("通讯失败！也许是您的网络傲娇了？");
							}
							
						);
					}
					
					
			
			
			
			
			
			var docHeight = $(window).height();
			$("body").css("min-height", docHeight);
			$(".container").css("min-height",docHeight);
			 	
			$("#inputID").show();
			$("#inputPassword").hide();
			$("#inputRole").hide();
			$("#getStart").hide();
			$(".Ok").hide();
			$("#selectID .selectLine").css("background-color","#3366FF");
			$("#selectID .textID h4 strong").css("color","#3366FF");
			
			//用户名点击事件
			$("#IDCheckImg").click(function(){
				//此处添加用户名的约束
				if($("#ID").val().length >= 7){
					$("#inputID").slideToggle(300,function(){
						$("#inputPassword").slideToggle(300);
					})
					$("#selectID .selectLine").css("background-color","#0ACF00");
					$("#selectID .textID h4 strong").css("color","#0ACF00");
					$("#selectPassword .selectLine").css("background-color","#3366FF");
					$("#selectPassword .textPassword h4 strong").css("color","#3366FF");
				}
				else{
					
				}
			});
			
			//密码点击事件
			$("#PasswordCheckImg").click(function(){
				//此处添加密码的约束
				if($("#ID").val().length != 0){
					$("#inputPassword").slideToggle(300,function(){
						$("#inputRole").slideToggle(300);
					})
					$("#selectPassword .selectLine").css("background-color","#0ACF00");
					$("#selectPassword .textPassword h4 strong").css("color","#0ACF00");
					$("#selectRole .selectLine").css("background-color","#3366FF");
					$("#selectRole .textRole h4 strong").css("color","#3366FF");
				}
				else{
					
				}
			});
			
			 //角色点击事件
			$("#RoleCheckImg").click(function(){
				if($(".radio label input[name = selectRole]:checked").val().toString() != null){
					$("#inputRole").slideToggle(300,function(){
						$("#getStart").slideToggle(300);
					})
					$("#selectRole .selectLine").css("background-color","#0ACF00");
					$("#selectRole .textRole h4 strong").css("color","#0ACF00");
				}
				else{
					
				}
			}); 
			
			//用户名状态回退
			$("#selectID").click(function(){
				if($("#inputPassword").is(":visible") == true){
					$("#inputPassword").slideToggle(300,function(){
						$("#inputID").slideToggle(300);
					})
					$("#selectID .selectLine").css("background-color","#3366FF");
					$("#selectID .textID h4 strong").css("color","#3366FF");
					$("#selectPassword .selectLine").css("background-color","#808080");
					$("#selectPassword .textPassword h4 strong").css("color","#808080");
					$("#selectRole .selectLine").css("background-color","#808080");
					$("#selectRole .textRole h4 strong").css("color","#808080");
				}
				else if($("#inputRole").is(":visible") == true){
					$("#inputRole").slideToggle(300,function(){
						$("#inputID").slideToggle(300);
					})
					$("#selectID .selectLine").css("background-color","#3366FF");
					$("#selectID .textID h4 strong").css("color","#3366FF");
					$("#selectPassword .selectLine").css("background-color","#808080");
					$("#selectPassword .textPassword h4 strong").css("color","#808080");
					$("#selectRole .selectLine").css("background-color","#808080");
					$("#selectRole .textRole h4 strong").css("color","#808080");
				}
			});
			
			//密码状态回退
			$("#selectPassword").click(function(){
				if($("#inputRole").is(":visible") == true){
					$("#inputRole").slideToggle(300,function(){
						$("#inputPassword").slideToggle(300);
					})
					$("#selectID .selectLine").css("background-color","#0ACF00");
					$("#selectID .textID h4 strong").css("color","#0ACF00");
					$("#selectPassword .selectLine").css("background-color","#3366FF");
					$("#selectPassword .textPassword h4 strong").css("color","#3366FF");
					$("#selectRole .selectLine").css("background-color","#808080");
					$("#selectRole .textRole h4 strong").css("color","#808080");
				}
			});	
				
		});
		
		
		
		
		
		
		
		
		
		
		 	
	}
	
);


			


