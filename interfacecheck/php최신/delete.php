<?php  
error_reporting(E_ALL); 
ini_set('display_errors',1); 

$link=mysqli_connect("localhost","interface518","518interface","interface518" ); 
if (!$link)  
{  
    echo "MySQL ���� ���� : ";
    echo mysqli_connect_error();
    exit();  
}  


mysqli_set_charset($link,"utf-8"); 

$test=isset($_POST['id']) ? $_POST['id'] : '';  

if ($test !=""){   
  
    $sql="DELETE  FROM Person WHERE id = '$test' LIMIT 10"; 
    $result=mysqli_query($link,$sql);   

    if($result){  
       echo "���������� �����Ǿ����ϴ�.";  
    }  
    else{  
       echo "SQL�� ó���� ���� �߻� : "; 
       echo mysqli_error($link);
    } 
 
} else {
    echo "�����͸� �Է��ϼ��� ";
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
         ������ ���ϴ� �й��� �Է��Ͻÿ�<br>
	 �й� : <input type = "text" name = "id" />
         <input type = "submit" />
      </form>  
   </body>
</html>
<?php
}
?>