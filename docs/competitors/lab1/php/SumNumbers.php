<?php
declare(strict_types=1);

define('NUMBER_DELIMITER', ' ');
define('INPUT_MESSAGE', "Введите числа через пробел: ");
define('RESULT_MESSAGE', "Сумма введенных чисел: ");
define('NEW_LINE', "\n");

//что делать, если пользователь ввел не число? 
//ошибка и прекращение работы или просто пропуск?
//я сделала просто пропуск
function main() 
{
    echo INPUT_MESSAGE;
    $numbers = readNumbers();
    $sum = 0;

    foreach ($numbers as $number) 
    {
        if (is_numeric($number)) 
        {
            $sum += floatval($number);
        }
    }
    echo RESULT_MESSAGE . $sum . NEW_LINE;
}

/**
 * @return string[]
 */
function readNumbers(): array 
{
    $input = trim(fgets(STDIN));
    return explode(NUMBER_DELIMITER, $input);
}

main();
?>