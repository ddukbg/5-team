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

if ($test !=""){   
  
    $sql="DELETE  FROM Person WHERE id = '$test' LIMIT 10"; 
    $result=mysqli_query($link,$sql);   

    if($result){  
       echo "성공적으로 삭제되었습니다.";  
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
         삭제를 원하는 학번을 입력하시오<br>
	 학번 : <input type = "text" name = "id" />
         <input type = "submit" />
      </form>  
   </body>
</html>
<?php
}
?>