<?php  
error_reporting(E_ALL); 
ini_set('display_errors',1); 

$link=mysqli_connect("localhost","interface518","518interface","interface518" ); 
if (!$link)  
{  
    echo "MySQL 접속 에러 : ";
    echo mysqli_connect_error();
    exit();  
}  


mysqli_set_charset($link,"utf-8"); 

$test=isset($_POST['id']) ? $_POST['id'] : '';  
$name=isset($_POST['name']) ? $_POST['name'] : '';  
$phone=isset($_POST['phone']) ? $_POST['phone'] : '';  
$department=isset($_POST['department']) ? $_POST['department'] : ''; 

if ($test !="" and $name !="" and $phone !=""){   
  
    $sql="UPDATE Person SET name = '$name', phone = '$phone', department = '$department' WHERE id = '$test' LIMIT 10"; 
    $result=mysqli_query($link,$sql);   

    if($result){  
       echo "SQL문 처리 성공";  
    }  
    else{  
       echo "SQL문 처리중 에러 발생 : "; 
       echo mysqli_error($link);
    } 
 
} else {
    echo "데이터를 입력하세요 ";
}


mysqli_close($link);
?>

<?php

$android = strpos($_SERVER['HTTP_USER_AGENT'], "Android");

if (!$android){
?>

<html>
   <body>
   
      <form action="<?php $_PHP_SELF ?>" method="POST">
         변경을 원하는 학번을 입력하시오<br>
	 학번 : <input type = "text" name = "id" /> <br> <br>
         변경하고싶은 값을 입력하고 제출을 누르시오<br>
	 이름 : <input type = "text" name = "name" />
         전화번호 : <input type = "text" name = "phone" />
	 학과 : <input type = "text" name = "department" />
         <input type = "submit" />
      </form>
   
   </body>
</html>
<?php
}
?>

