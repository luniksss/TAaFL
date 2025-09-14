package main

import (
	"bufio"
	"fmt"
	"math"
	"os"
	"strconv"
	"strings"
)

const (
	inputMessage      = "Введите радиус круга: "
	resultMessage     = "Площадь вашего круга равна: %f\n"
	errorMessage      = "Пожалуйста, введите корректное положительное число для радиуса.\n"
	
	stopReadingSymbol = '\n'
	minRadius         = 0
	bitPrecision      = 64
)

func main() {
	inputStream := bufio.NewReader(os.Stdin)

	fmt.Printf(inputMessage)
	input, _ := inputStream.ReadString(stopReadingSymbol)
	inputRadius := strings.TrimSpace(input)

	radius, err := strconv.ParseFloat(inputRadius, bitPrecision)
	if err != nil || radius <= minRadius {
		fmt.Printf(errorMessage)
		return
	}

	area := math.Pi * math.Pow(radius, 2)
	fmt.Printf(resultMessage, area)
}
