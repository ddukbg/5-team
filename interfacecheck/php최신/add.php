<?php  
error_reporting(E_ALL); 
ini_set('display_errors',1); 

$link=mysqli_connect("localhost","interface518","518interface","interface518"); 
if (!$link)  
{ 
   echo "MySQL ���� ���� : ";
   echo mysqli_connect_error();
   exit();
}  


mysqli_set_charset($link,"utf8");  

//POST ���� �о�´�.
$id=isset($_POST['id']) ? $_POST['id'] : '';  
$name=isset($_POST['name']) ? $_POST['name'] : '';  
$phone=isset($_POST['phone']) ? $_POST['phone'] : '';  
$department=isset($_POST['department']) ? $_POST['department'] : ''; 
 

if ($id !="" and $name !="" and $phone !=""){   
  
    $sql="insert into Person(id, name, phone, department) values('$id','$name','$phone','$department')";  
    $result=mysqli_query($link,$sql);  

    if($result){  
       echo "SQL�� ó�� ����";  
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
         �й� : <input type = "text" name = "id" />
	 �̸� : <input type = "text" name = "name" />
         ��ȭ��ȣ : <input type = "text" name = "phone" />
	 �а� : <input type = "text" name = "department" />
         <input type = "submit" />
      </form>
   
   </body>
</html>
<?php
}
?>
