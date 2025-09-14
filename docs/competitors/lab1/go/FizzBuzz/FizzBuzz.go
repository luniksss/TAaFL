package main

import (
	"bufio"
	"fmt"
	"os"
	"strconv"
	"strings"
)

const (
	firstMessage = "Введите целое число (для завершения введите 'stop')"
	errorInputMessage = "Пожалуйста, введите целое число или 'stop' для завершения"
	inputSymbol  = ": "
	stopWord     = "stop"

	newLineSymbol = "\n"
	divisibleBy3  = "Fizz"
	divisibleBy5  = "Buzz"
)

func main() {
	fmt.Println(firstMessage)
	scanner := bufio.NewScanner(os.Stdin)

	for {
		fmt.Print(inputSymbol)
		scanner.Scan()
		input := strings.TrimSpace(scanner.Text())

		if strings.ToLower(input) == stopWord {
			break
		}

		if validateInput(input) {
			fmt.Println(errorInputMessage)
			continue
		}

		num, _ := strconv.Atoi(input)
		fmt.Print(defineResultMessage(num))
	}
}

func validateInput(input string) bool {
	_, err := strconv.Atoi(input)
	return err != nil 
}

func defineResultMessage(number int) string {
	dividedBy3 := canBeDivided(number, 3)
	dividedBy5 := canBeDivided(number, 5)
	resultMessage := ""

	if dividedBy3 {
		resultMessage += divisibleBy3
	}

	if dividedBy5 {
		resultMessage += divisibleBy5
	}

	if (!dividedBy3 && !dividedBy5) {
		resultMessage += strconv.Itoa(number)
	}

	resultMessage += newLineSymbol
	return resultMessage
}

func canBeDivided(dividend, divider int) bool {
	return dividend%divider == 0
}