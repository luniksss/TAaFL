<?php
declare(strict_types=1);

define('INPUT_MESSAGE', "Введите целое число (для завершения введите 'stop')");
define('ERROR_MESSAGE', "Пожалуйста, введите целое число или 'stop' для завершения");
define('INPUT_SYMBOL', ": ");
define('STOP_WORD', "stop");
define('DIVISIBLE_BY_3', "Fizz");
define('DIVISIBLE_BY_5', "Buzz");
define('NEW_LINE', "\n");

function main() 
{
    echo INPUT_MESSAGE . NEW_LINE;
    while (true)
    {
        echo INPUT_SYMBOL;
        $input = trim(fgets(STDIN));

        if (strtolower($input) === STOP_WORD)
        {
            break;
        }

        if (validateInput($input)) 
        {
            echo ERROR_MESSAGE . NEW_LINE;
            continue;
        }

        $inputNumber = intval($input);
        echo defineResultMessage($inputNumber);
    }
}

function validateInput(string $input): bool
{
    return empty($input) 
        || !is_numeric($input) 
        || strpos($input, '.') !== false;
}

function defineResultMessage(int $number): string
{
    $canBeDivided3 = canBeDivided($number, 3);
    $canBeDivided5 = canBeDivided($number, 5);
    $resultMessage = "";

    if ($canBeDivided3) 
    {
        $resultMessage .= DIVISIBLE_BY_3;
    } 
    
    if ($canBeDivided5) 
    {
        $resultMessage .= DIVISIBLE_BY_5;
    } 

    if (!$canBeDivided3 && !$canBeDivided5) 
    {
        $resultMessage .= $number;
    }

    return $resultMessage .= NEW_LINE;
}

function canBeDivided(int $dividend, int $divider): bool
{
    return $dividend % $divider === 0;
}

main()
?>