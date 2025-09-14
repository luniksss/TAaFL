<?php
declare(strict_types=1);

define('INPUT_MESSAGE', "Введите радиус круга: ");
define('RESULT_MESSAGE', "Площадь вашего круга равна: %s\n");
define('ERROR_MESSAGE', "Пожалуйста, введите корректное положительное число для радиуса.\n");
define('MIN_RADIUS', 0);

echo INPUT_MESSAGE;
$radius = trim(fgets(STDIN));

if (is_numeric($radius) && $radius > MIN_RADIUS) 
{
    $area = pi() * $radius * $radius;
    printf(RESULT_MESSAGE, $area);
} 
else 
{
    echo ERROR_MESSAGE;
}
?>