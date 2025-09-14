package main

import (
	"bufio"
	"fmt"
	"os"
	"strconv"
	"strings"
)

const (
	inputMessage    = "Введите числа через пробел: "
	resultMessage   = "Сумма введенных чисел: "
	newLineSymbol   = '\n'
	numberDelimiter = " "
)

func main() {
	fmt.Print(inputMessage)
	numbers := readNumbers()
	sum := 0.0

	for _, numberStr := range numbers {
		if number, err := strconv.ParseFloat(numberStr, 64); err == nil {
			sum += number
		}
	}
	
	fmt.Printf("%s%.2f\n", resultMessage, sum)
}

func readNumbers() []string {
	reader := bufio.NewReader(os.Stdin)

	input, _ := reader.ReadString(newLineSymbol)
	input = strings.TrimSpace(input)
	
	return strings.Split(input, numberDelimiter)
}